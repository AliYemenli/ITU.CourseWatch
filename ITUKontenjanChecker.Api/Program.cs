using ITUKontenjanChecker.Api.Data;
using ITUKontenjanChecker.Api.Endpoints;
using ITUKontenjanChecker.Api.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("KontenjanChecker");

builder.Services.AddSqlite<KontenjanCheckerContext>(connString);
builder.Services.AddScoped<KontenjanCheckerContext>();

builder.Services.AddHostedService<BranchUpdaterService>();
builder.Services.AddHostedService<CourseUpdaterService>();
builder.Services.AddHostedService<AlarmCheckerWorker>();


var app = builder.Build();

app.MapBranchesEndpoints();
app.MapCoursesEndpoints();
app.MapAlarmsEndpoints();



app.MigrateDb();
app.Run();
