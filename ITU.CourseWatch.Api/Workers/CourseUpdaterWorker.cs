using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }

            await Task.Delay(1000 * 10);
        }
    }
}

