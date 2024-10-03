using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;

namespace ITU.CourseWatch.Api.Workers;

public class BranchUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BranchService _branchService = new BranchService();

    public BranchUpdaterService(IServiceProvider serviceProvider)
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

                    await _branchService.UpdateBranchesAsync(dbContext);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                throw;
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
