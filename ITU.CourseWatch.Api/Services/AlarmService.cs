using ITU.CourseWatch.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using ITU.CourseWatch.Api.Entities;
using ITU.CourseWatch.Api.Data;
using Serilog;

namespace ITU.CourseWatch.Api.Services;

public class AlarmService
{
    private readonly CourseWatchContext _dbContext;
    private readonly MailService _mailService;


    public AlarmService(CourseWatchContext CourseWatchContext)
    {
        _dbContext = CourseWatchContext;
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
        return alarm.Course.Capacity > alarm.Course.Enrolled;
    }

    public async Task SendAlarmAsync()
    {
        var alarms = await GetAlarmsAsync();

        List<Task<Alarm>> mailTasks = new List<Task<Alarm>>();

        try
        {
            foreach (var alarm in alarms)
            {
                if (IsAvailable(alarm))
                {
                    mailTasks.Add(_mailService.SendAlarmMailAsync(alarm));
                }
            }

            while (mailTasks.Count > 0)
            {
                var finishedAlarm = await Task.WhenAny(mailTasks);

                _dbContext.Alarms.Remove(await finishedAlarm);
            }
        }
        catch (Exception e)
        {
            Log.Fatal(" [{Class}] Error occured. Exception:. Exception: {Exception}", this, e.Message);
        }

        await _dbContext.SaveChangesAsync();
    }


}
