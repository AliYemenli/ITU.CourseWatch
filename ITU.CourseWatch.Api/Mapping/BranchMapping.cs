using ITU.CourseWatch.Api.Dtos;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Mapping;

public static class BranchMapping
{
    public static BranchSummaryDto ToBranchSummaryDto(this Branch branch)
    {
        return new BranchSummaryDto(branch.BranchCode);
    }
}
