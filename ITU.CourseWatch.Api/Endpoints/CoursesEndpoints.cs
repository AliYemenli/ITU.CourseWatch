using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Mapping;
using ITU.CourseWatch.Api.Repository.BranchRepositories;
using ITU.CourseWatch.Api.Repository.CourseRepositories;
using Microsoft.EntityFrameworkCore;

namespace ITU.CourseWatch.Api.Endpoints;

public static class CoursesEndpoints
{
    public static RouteGroupBuilder MapCoursesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("courses")
            .WithParameterValidation();


        // GET //courses
        group.MapGet("/", async (ICourseRepository courseRepository) =>
        {
            var courses = await courseRepository.GetAllAsync();
            return courses.Select(c => c.ToCourseSummaryDto());
        });

        // GET //courses/crn/{crn}
        group.MapGet("/crn/{crn}", async (string crn, ICourseRepository courseRepository) =>
        {
            var course = await courseRepository.GetAsync(crn);

            return course is null ?
                Results.NotFound("There is no course for given CRN") : Results.Ok(course.ToCourseSummaryDto());
        })
        .WithName("GetCourseByCrn");

        // GET //courses/branch/{branchCode}
        group.MapGet("/branch/{branchCode}", async (string branchCode, ICourseRepository courseRepository) =>
        {
            var coursesList = await courseRepository.GetAllAsync();
            var courses = coursesList
                                .Where(c => c.Branch.BranchCode == branchCode)
                                .ToList();


            return courses.Count == 0 ?
                Results.NotFound("There is no course for given BranchCode") : Results.Ok(courses.ToCourseSummaryDto());
        })
        .WithName("GetCoursesOfBranch");

        return group;
    }
}
