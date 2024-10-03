using System;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ITUKontenjanChecker.Api.Endpoints;

public static class CoursesEndpoints
{
    public static RouteGroupBuilder MapCoursesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("courses")
            .WithParameterValidation();


        // GET //courses
        group.MapGet("/", async (KontenjanCheckerContext dbContext) =>
            await dbContext.Courses
                .Include(c => c.Branch)
                .Select(c => c.ToCourseSummaryDto())
                .AsNoTracking()
                .ToListAsync()
        );

        // GET //courses/crn/{crn}
        group.MapGet("/crn/{crn}", async (string crn, KontenjanCheckerContext dbContext) =>
        {
            var course = await dbContext.Courses
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Crn == crn);

            return course is null ?
                Results.NotFound("There is no course for given CRN") : Results.Ok(course.ToCourseSummaryDto());
        })
        .WithName("GetCourseByCrn");

        // GET //courses/branch/{branchCode}
        group.MapGet("/branch/{branchCode}", async (string branchCode, KontenjanCheckerContext dbContext) =>
        {
            var courses = await dbContext.Courses
                .Include(c => c.Branch)
                .AsNoTracking()
                .Where(c => c.Branch.BranchCode == branchCode)
                .ToListAsync();

            return courses.Count == 0 ?
                Results.NotFound("There is no course for given BranchCode") : Results.Ok(courses.ToCourseSummaryDto());
        })
        .WithName("GetCoursesOfBranch");

        return group;
    }
}
