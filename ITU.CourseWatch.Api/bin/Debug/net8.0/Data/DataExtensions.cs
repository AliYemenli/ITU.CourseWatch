using ITU.CourseWatch.Api.Endpoints;
using ITU.CourseWatch.Api.Workers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Enrichers.WithCaller;

namespace ITU.CourseWatch.Api.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CourseWatchContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static void AddWorkers(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<BranchUpdaterService>();
        builder.Services.AddHostedService<CourseUpdaterService>();
        builder.Services.AddHostedService<AlarmCheckerWorker>();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapBranchesEndpoints();
        app.MapCoursesEndpoints();
        app.MapAlarmsEndpoints();
    }

    public static void SetDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("CourseWatch");
        builder.Services.AddSqlite<CourseWatchContext>(connString);
        builder.Services.AddScoped<CourseWatchContext>();
    }

    public static void SetSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithCaller()
            .MinimumLevel.Warning()
            .WriteTo.Console()
            .WriteTo.File
                (
                    "Logs/CourseWatchLogs-.txt",
                    rollingInterval: RollingInterval.Month
                )
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();
    }
}