using ITU.CourseWatch.Api.Repository.BranchRepositories;
using ITU.CourseWatch.Api.Services;
using Serilog;

namespace ITU.CourseWatch.Api.Workers;

public class BranchUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

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
                    var branchService = scope.ServiceProvider.GetRequiredService<BranchService>();
                    await branchService.RefreshBranchesAsync();
                }
            }
            catch (Exception e)
            {
                Log.Fatal(" [{Class}] Error occured at Branch Worker. Exception: Exception: {Exception}", nameof(BranchUpdaterService), e.Message);
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

}
