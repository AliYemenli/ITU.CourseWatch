using System;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Services;

namespace ITUKontenjanChecker.Api.Workers;

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
                    var dbContext = scope.ServiceProvider.GetRequiredService<KontenjanCheckerContext>();
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
