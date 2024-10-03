using System;
using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Services;

namespace ITUKontenjanChecker.Api.Workers;

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
                    var dbContext = scope.ServiceProvider.GetRequiredService<KontenjanCheckerContext>();

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
