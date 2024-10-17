using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Mapping;
using ITU.CourseWatch.Api.Repository.BranchRepositories;
using ITU.CourseWatch.Api.Repository.CourseRepositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ITU.CourseWatch.Api.Services;

public class CourseService
{
    private const string Url = "https://obs.itu.edu.tr/public/DersProgram/DersProgramSearch?ProgramSeviyeTipiAnahtari=LS&dersBransKoduId=";
    private readonly ICourseRepository _courseRepository;
    private readonly IBranchRepository _branchRepository;

    public CourseService(ICourseRepository courseRepository, IBranchRepository branchRepository)
    {
        _courseRepository = courseRepository;
        _branchRepository = branchRepository;
    }

    public async Task<CourseResponseDto> FetchCoursesAsync(string branch)
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

    private async Task<List<CourseDto>> FetchCourseDtosAsync(string branch)
    {
        var courses = await FetchCoursesAsync(branch);
        return await courses.ToCourseDtoListAsync();
    }

    private async Task<List<Course>> GetCourseEntitiesAsync()
    {
        var courseEntities = new ConcurrentBag<Course>();
        var branches = await _branchRepository.GetAllAsync();

        List<Task> courseFetchTasks = new List<Task>();
        try
        {
            foreach (var branch in branches)
            {
                var courseEntitiyTask = FetchCourseDtosAsync(branch.BranchId.ToString()).ContinueWith(
                    (finishedTask) =>
                    {
                        foreach (var course in finishedTask.Result)
                        {
                            courseEntities.Add(new Course
                            {
                                Crn = course.Crn,
                                Code = course.CourseCode,
                                Name = course.CourseName,
                                Instructor = course.CourseInstructor,
                                Capacity = course.CourseCapacity,
                                Enrolled = course.CourseEnrolled,
                                BranchId = branch.BranchId,
                                Branch = branch
                            });
                        }
                    }
                );

                courseFetchTasks.Add(courseEntitiyTask);


            }
            await Task.WhenAll(courseFetchTasks);
        }
        catch (Exception e)
        {
            Log.Error(" [{Class}] Error at the parsing the dto to entities, chance to return a empty list. Exception:. Exception: {Exception}", this, e.Message);
        }

        return courseEntities.ToList();
    }


    public async Task RefreshCoursesAsync()
    {
        var newCourses = await GetCourseEntitiesAsync();
        List<Task> courseTasks = new List<Task>();
        var coursesToCreate = new ConcurrentBag<Course>();
        var coursesToUpdate = new ConcurrentBag<Course>();

        try
        {
            foreach (var course in newCourses)
            {
                var existingCourseTask = _courseRepository.GetAsync(course);



                courseTasks.Add(existingCourseTask.ContinueWith(async existingTask =>
                {
                    var getCourseTask = await existingTask;

                    if (getCourseTask is null)
                    {
                        coursesToCreate.Add(course);
                    }
                    else
                    {
                        // Create a local copy of course to avoid mutation issues
                        var courseToUpdate = new Course
                        {
                            Crn = course.Crn,
                            Code = course.Code,
                            Name = course.Name,
                            Instructor = course.Instructor,
                            Capacity = course.Capacity,
                            Enrolled = course.Enrolled,
                            BranchId = course.BranchId,
                            Branch = course.Branch,
                            Id = getCourseTask.Id // Update with existing ID
                        };
                        coursesToUpdate.Add(courseToUpdate);
                        // await _courseRepository.UpdateAsync(courseToUpdate);
                    }



                }));
            }

            await Task.WhenAll(courseTasks);

            await _courseRepository.CreateBulkAsync(coursesToCreate.ToList());
            await _courseRepository.UpdateBulkAsync(coursesToUpdate.ToList());

        }
        catch (Exception e)
        {
            Log.Fatal(" [{Class}] A problem occured when trying to update Course table of DB, require attention . Exception:. Exception: {Exception}", this, e.Message);
        }
    }



}
