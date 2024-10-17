using System;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Repository.BranchRepositories;

public interface IBranchRepository
{
    Task<Branch?> GetAsync(Branch branch);
    Task<Branch?> GetAsync(string branchCode);
    Task CreateAsync(Branch newBranch);
    Task<IEnumerable<Branch>> GetAllAsync();
    Task UpdateAsync(Branch updatedBranch);

}
