using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;
using Serilog;

namespace ITU.CourseWatch.Api.Workers;

public class CourseUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public CourseUpdaterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Course worker started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var courseService = scope.ServiceProvider.GetRequiredService<CourseService>();
                    await courseService.RefreshCoursesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Fatal(" [{Class}] Error occured at course worker. Exception: Exception: {Exception}", this, e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}

