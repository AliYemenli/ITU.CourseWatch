using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITU.CourseWatch.Api.Repository.BranchRepositories;

public class EFBranchRepository : IBranchRepository
{
    private readonly CourseWatchContext _dbContext;

    public EFBranchRepository(CourseWatchContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAsync(Branch newBranch)
    {
        _dbContext.Branches.Add(newBranch);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Branch>> GetAllAsync()
    {
        return await _dbContext.Branches
            .ToListAsync();
    }

    public async Task<Branch?> GetAsync(Branch branch)
    {
        return await _dbContext.Branches
            .FirstOrDefaultAsync(b => b.BranchCode == branch.BranchCode);
    }

    public async Task<Branch?> GetAsync(string branchCode)
    {
        return await _dbContext.Branches
            .FirstOrDefaultAsync(b => b.BranchCode == branchCode);
    }

    public async Task UpdateAsync(Branch updatedBranch)
    {
        _dbContext.Branches.Update(updatedBranch);
        await _dbContext.SaveChangesAsync();
    }
}
