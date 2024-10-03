using System;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Entities;
using ITUKontenjanChecker.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ITUKontenjanChecker.Api.Endpoints;

public static class BranchesEndpoint
{
    public static RouteGroupBuilder MapBranchesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("branches")
            .WithParameterValidation();

        //GET /branches
        group.MapGet("/", async (KontenjanCheckerContext dbContext) =>
            await dbContext.Branches
                .Select(b => b.ToBranchSummaryDto())
                .AsNoTracking()
                .ToListAsync()
        );

        //GET /branches/branchCode
        group.MapGet("/{branchCode}", async (string branchCode, KontenjanCheckerContext dbContext) =>
        {
            Branch? branch = await dbContext.Branches
                .FirstOrDefaultAsync(b => b.BranchCode.Equals(branchCode, StringComparison.CurrentCultureIgnoreCase));

            return branch is null ?
                Results.NotFound() : Results.Ok(branch.ToBranchSummaryDto());
        });


        return group;
    }
}
