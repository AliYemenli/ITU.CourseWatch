using System;
using ITU.CourseWatch.Api.Entities;

namespace ITU.CourseWatch.Api.Repository.AlarmRepositories;

public interface IAlarmRepository
{
    Task CreateAsync(Alarm alarm);
    Task<List<Alarm>> GetAllAsync();
    Task<List<Alarm>> GetAvailablesAsync();
    Task DeleteAsync(Alarm alarm);
    Task DeleteBulkAsync(List<Alarm> alarms);
}
