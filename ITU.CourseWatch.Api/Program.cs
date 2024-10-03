using ITU.CourseWatch.Api.Data;
using ITU.CourseWatch.Api.Endpoints;
using ITU.CourseWatch.Api.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("KontenjanChecker");

builder.Services.AddSqlite<CourseWatchContext>(connString);
builder.Services.AddScoped<CourseWatchContext>();

builder.Services.AddHostedService<BranchUpdaterService>();
builder.Services.AddHostedService<CourseUpdaterService>();
builder.Services.AddHostedService<AlarmCheckerWorker>();


var app = builder.Build();

app.MapBranchesEndpoints();
app.MapCoursesEndpoints();
app.MapAlarmsEndpoints();



app.MigrateDb();
app.Run();
