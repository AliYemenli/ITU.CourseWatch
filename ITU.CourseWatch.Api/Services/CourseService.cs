using System.Text.Json;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Mapping;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ITU.CourseWatch.Api.Services;

public class CourseService
{
    private const string Url = "https://obs.itu.edu.tr/public/DersProgram/DersProgramSearch?ProgramSeviyeTipiAnahtari=LS&dersBransKoduId=";

    public async Task<CourseResponseDto> GetCoursesAsync(string branch)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, Url + branch);

        CourseResponseDto responseDto = new CourseResponseDto();

        var response = new HttpResponseMessage();
        try
        {
            response = await client.SendAsync(request);
        }
        catch (Exception ex)
        {
            Log.Error(" [{Class}] API request failed. Exception:. Exception: {Exception}", this, ex.Message);
        }

        if (response is not null && response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

                    var responseDtoTask = await JsonSerializer.DeserializeAsync<CourseResponseDto>(stream);
                    responseDto = responseDtoTask ?? new();
                }
                catch (Exception e)
                {
                    Log.Error(" [{Class}] Problem with the Deserializing the api response. Exception:. Exception: {Exception}", this, e.Message);
                }
            }
            else
            {
                Log.Error(" [{Class}] API response is 200 but content is empty or null somehow. Exception:. Exception: {Exception}", this);
            }
        }

        return responseDto;
    }

    private async Task<List<CourseDto>> GetCourseDtosAsync(string branch)
    {
        var courses = await GetCoursesAsync(branch);
        return await courses.ToCourseDtoListAsync();
    }

    private async Task<List<Course>> GetCourseEntitiesAsync(CourseWatchContext dbContext)
    {
        List<Course> courseEntities = new List<Course>();

        try
        {
            foreach (var branch in await dbContext.Branches.ToListAsync())
            {
                var branchCourses = await GetCourseDtosAsync(branch.BranchId.ToString());

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
        }
        catch (Exception e)
        {
            Log.Error(" [{Class}] Error at the parsing the dto to entities, chance to return a empty list. Exception:. Exception: {Exception}", this, e.Message);
        }

        return courseEntities;
    }


    public async Task UpdateCoursesAsync(CourseWatchContext dbContext)
    {
        var newCourses = await GetCourseEntitiesAsync(dbContext);
        List<Task> courseTasks = new();

        try
        {
            foreach (var course in newCourses)
            {
                var existingCourseTask = dbContext.Courses
                    .Where(z => z.Crn == course.Crn)
                    .FirstOrDefaultAsync();

                courseTasks.Add(existingCourseTask.ContinueWith(existingTask =>
                {
                    var existingCourse = existingTask.Result;
                    if (existingCourse is null)
                    {
                        dbContext.Courses.AddAsync(course);
                    }
                    else
                    {
                        existingCourse.Equals(course);
                    }
                }));
            }
        }
        catch (Exception e)
        {
            Log.Fatal(" [{Class}] A problem occured when trying to update Course table of DB, require attention . Exception:. Exception: {Exception}", this, e.Message);
        }
        await Task.WhenAll(courseTasks);
        await dbContext.SaveChangesAsync();
    }



}
