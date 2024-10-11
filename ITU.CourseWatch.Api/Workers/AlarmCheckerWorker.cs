using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;
using Serilog;

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
        Log.Information("Alarm worker started.");
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
            catch (Exception e)
            {
                Log.Fatal(" [{Class}] Error occured at alarm worker. Exception: Exception: {Exception}", this, e.Message);

            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}
