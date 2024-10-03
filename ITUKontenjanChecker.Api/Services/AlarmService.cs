using System;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Entities;
using ITUKontenjanChecker.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ITUKontenjanChecker.Api.Services;

public class AlarmService
{
    private readonly KontenjanCheckerContext _dbContext;
    private readonly MailService _mailService;


    public AlarmService(KontenjanCheckerContext kontenjanCheckerContext)
    {
        _dbContext = kontenjanCheckerContext;
        _mailService = new MailService();
    }
    private async Task<List<Alarm>> GetAlarmsAsync()
    {
        return await _dbContext.Alarms
        .Include(a => a.Course)
        .Include(a => a.Course.Branch)
        .AsNoTracking()
        .ToListAsync() ?? new List<Alarm>();
    }

    private bool IsAvailable(Alarm alarm)
    {
        return alarm.Course is not null && alarm.Course.Capacity > alarm.Course.Enrolled;
    }

    public async Task SendAlarmAsync()
    {
        var alarms = await GetAlarmsAsync();

        foreach (var alarm in alarms)
        {
            if (IsAvailable(alarm))
            {
                _mailService.SendEmail(new MailBodyDto(
                    alarm.Subscriber,
                    "Course Availability Notification",
                    _mailService.GetAlarmBody(alarm)
                ));

                await _dbContext.Alarms.Where(a => a == alarm).ExecuteDeleteAsync();
            }
        }
    }


}
