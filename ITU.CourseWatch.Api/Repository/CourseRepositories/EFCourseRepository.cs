using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITU.CourseWatch.Api.Repository.CourseRepositories;

public class EFCourseRepository : ICourseRepository
{
    private readonly CourseWatchContext _dbContext;
    public EFCourseRepository(CourseWatchContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateAsync(Course course)
    {
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _dbContext.Courses
            .Include(c => c.Branch)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<Course?> GetAsync(Course course)
    {
        return await _dbContext.Courses
                                .Include(c => c.Branch)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Crn == course.Crn);
    }
    public async Task<Course?> GetAsync(string crn)
    {
        return await _dbContext.Courses
                        .Include(c => c.Branch)
                        .FirstOrDefaultAsync(c => c.Crn == crn);
    }


    public async Task UpdateAsync(Course course)
    {
        _dbContext.Courses.Update(course);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateBulkAsync(IEnumerable<Course> courses)
    {
        if (courses == null || !courses.Any())
        {
            await _dbContext.SaveChangesAsync();
            return;
        }

        await _dbContext.Courses.AddRangeAsync(courses);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateBulkAsync(IEnumerable<Course> courses)
    {
        await Task.Run(() => _dbContext.Courses.UpdateRange(courses));
        await _dbContext.SaveChangesAsync();
    }

}

