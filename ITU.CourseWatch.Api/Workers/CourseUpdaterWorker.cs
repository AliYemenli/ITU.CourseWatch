using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;
using Serilog;

namespace ITU.CourseWatch.Api.Workers;

public class CourseUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CourseService _courseService = new CourseService();

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
                    var dbContext = scope.ServiceProvider.GetRequiredService<CourseWatchContext>();
                    await _courseService.UpdateCoursesAsync(dbContext);
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

