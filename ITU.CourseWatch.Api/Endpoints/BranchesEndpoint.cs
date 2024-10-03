using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ITU.CourseWatch.Api.Endpoints;

public static class BranchesEndpoint
{
    public static RouteGroupBuilder MapBranchesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("branches")
            .WithParameterValidation();

        //GET /branches
        group.MapGet("/", async (CourseWatchContext dbContext) =>
            await dbContext.Branches
                .Select(b => b.ToBranchSummaryDto())
                .AsNoTracking()
                .ToListAsync()
        );

        //GET /branches/branchCode
        group.MapGet("/{branchCode}", async (string branchCode, CourseWatchContext dbContext) =>
        {
            Branch? branch = await dbContext.Branches
                .FirstOrDefaultAsync(b => b.BranchCode.Equals(branchCode, StringComparison.CurrentCultureIgnoreCase));

            return branch is null ?
                Results.NotFound() : Results.Ok(branch.ToBranchSummaryDto());
        });


        return group;
    }
}
