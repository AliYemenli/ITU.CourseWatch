using System;
using ITUKontenjanChecker.Api.Dtos;
using ITUKontenjanChecker.Api.Entities;

namespace ITUKontenjanChecker.Api.Mapping;

public static class BranchMapping
{
    public static BranchSummaryDto ToBranchSummaryDto(this Branch branch)
    {
        return new BranchSummaryDto(branch.BranchCode);
    }
}
