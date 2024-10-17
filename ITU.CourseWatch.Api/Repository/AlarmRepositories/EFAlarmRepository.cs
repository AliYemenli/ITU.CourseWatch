namespace ITU.CourseWatch.Api.Repository.AlarmRepositories;
using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Entities;
using Microsoft.EntityFrameworkCore;


public class EFAlarmRepository : IAlarmRepository
{
    private readonly CourseWatchContext _dbContext;

    public EFAlarmRepository(CourseWatchContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task CreateAsync(Alarm alarm)
    {
        await _dbContext.Alarms.AddAsync(alarm);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Alarm alarm)
    {
        await _dbContext.Alarms
                .Where(a => a == alarm)
                .ExecuteDeleteAsync();
    }

    public async Task DeleteBulkAsync(List<Alarm> alarms)
    {
        await Task.Run(() => _dbContext.Alarms.RemoveRange(alarms));
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Alarm>> GetAllAsync()
    {
        return await _dbContext.Alarms
        .Include(a => a.Course)
        .Include(a => a.Course.Branch)
        .ToListAsync() ?? new List<Alarm>();
    }

    public async Task<List<Alarm>> GetAvailablesAsync()
    {
        return await _dbContext.Alarms
        .Include(a => a.Course)
        .Include(a => a.Course.Branch)
        .Where(alarm => alarm.Course.Capacity > alarm.Course.Enrolled)
        .ToListAsync();
    }
}
