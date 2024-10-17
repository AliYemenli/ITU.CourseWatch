using ITU.CourseWatch.Api.Endpoints;
using ITU.CourseWatch.Api.Repository.AlarmRepositories;
using ITU.CourseWatch.Api.Repository.BranchRepositories;
using ITU.CourseWatch.Api.Repository.CourseRepositories;
using ITU.CourseWatch.Api.Services;
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
        builder.Services
            .AddHostedService<BranchUpdaterService>()
            .AddHostedService<CourseUpdaterService>()
            .AddHostedService<AlarmCheckerWorker>();
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapBranchesEndpoints();
        app.MapCoursesEndpoints();
        app.MapAlarmsEndpoints();
    }



    public static IServiceCollection AddRepositories(
            this IServiceCollection services,
            IConfiguration configuration
        )
    {
        var connString = configuration.GetConnectionString("CourseWatch");
        services.AddSqlite<CourseWatchContext>(connString)
                .AddScoped<CourseWatchContext>()
                .AddScoped<BranchService>()
                .AddScoped<IBranchRepository, EFBranchRepository>()
                .AddScoped<CourseService>()
                .AddScoped<ICourseRepository, EFCourseRepository>()
                .AddScoped<AlarmService>()
                .AddScoped<IAlarmRepository, EFAlarmRepository>();

        return services;
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