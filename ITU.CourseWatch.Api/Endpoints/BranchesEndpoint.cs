namespace ITU.CourseWatch.Api.Endpoints;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Mapping;
using ITU.CourseWatch.Api.Repository.BranchRepositories;
using Serilog;

public static class BranchesEndpoint
{
    public static RouteGroupBuilder MapBranchesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("branches")
            .WithParameterValidation();

        //GET /branches
        group.MapGet("/", async (IBranchRepository branchRepository) =>
        {

            var branches = await branchRepository.GetAllAsync();

            return branches.Select(branch => branch.ToBranchSummaryDto());
        });

        //GET /branches/branchCode
        group.MapGet("/{branchCode}", async (string branchCode, IBranchRepository branchRepository) =>
        {
            Branch? branch = null;
            try
            {
                branch = await branchRepository.GetAsync(branchCode);
            }
            catch (Exception e)
            {
                Log.Error(" [{Class}] Error occured at /branches/branchCode. Exception: {Exception}", nameof(MapBranchesEndpoints), e.Message);
                return Results.Problem("Internal Server error", statusCode: 500);
            }
            return branch is null ?
                Results.NotFound() : Results.Ok(branch.ToBranchSummaryDto());
        });


        return group;
    }
}
