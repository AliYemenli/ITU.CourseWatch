using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;

namespace ITU.CourseWatch.Api.Workers;

public class AlarmCheckerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;


    public AlarmCheckerWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CourseWatchContext>();
                    AlarmService alarmService = new AlarmService(dbContext);

                    await alarmService.SendAlarmAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }

            await Task.Delay(1000 * 5);
        }
    }
}
