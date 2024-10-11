using System;
using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Services;
using Serilog;

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
        Log.Information("Branch worker started.");
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
            catch (Exception e)
            {
                Log.Fatal(" [{Class}] Error occured at Branch Worker. Exception: Exception: {Exception}", this, e.Message);
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
