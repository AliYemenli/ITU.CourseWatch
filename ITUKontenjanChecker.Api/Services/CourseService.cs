using System;
using System.Text.Json;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;

namespace ITUKontenjanChecker.Api.Services;

public class CourseService
{
    private const string _url = "https://obs.itu.edu.tr/public/DersProgram/DersProgramSearch?ProgramSeviyeTipiAnahtari=LS&dersBransKoduId=";

    public async Task<CourseResponseDto> GetAsyncCourseClient(string branch)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, _url + branch);

        CourseResponseDto responseDto = new CourseResponseDto();

        var response = new HttpResponseMessage();
        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception("API request failed. Exception: " + ex.Message);
        }

        if (response is not null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                responseDto = JsonSerializer.Deserialize<CourseResponseDto>(content) ?? new CourseResponseDto();
            }
        }
        return responseDto;
    }

    private List<CourseDto> GetAllCoursesOfBranch(string branch)
    {
        var courses = GetAsyncCourseClient(branch).Result;
        return courses.GetAsCourseDto();
    }

    private Task<List<Course>> GetAllCoursesAsEntities(KontenjanCheckerContext dbContext)
    {
        List<Course> courseEntities = new List<Course>();
        foreach (var branch in dbContext.Branches.ToList())
        {
            var branchCourses = GetAllCoursesOfBranch(branch.BranchId.ToString());

            foreach (var course in branchCourses)
            {
                courseEntities.Add(new Course
                {
                    Crn = course.Crn,
                    Code = course.CourseCode,
                    Name = course.CourseName,
                    Instructor = course.CourseInstructor,
                    Capacity = course.CourseCapacity,
                    Enrolled = course.CourseEnrolled,
                    BranchId = branch.Id,
                    Branch = branch
                });
            }
        }

        return Task.FromResult(courseEntities);
    }


    public async Task UpdateCoursesAsync(KontenjanCheckerContext dbContext)
    {
        var newCourses = await GetAllCoursesAsEntities(dbContext);

        foreach (var course in newCourses)
        {
            var existingCourse = dbContext.Courses.Where(z => z.Crn == course.Crn).FirstOrDefault();
            if (existingCourse is null)
            {
                dbContext.Courses.Add(course);
            }
            else
            {
                existingCourse.Equals(course);
            }
        }
        await dbContext.SaveChangesAsync();
    }



}
