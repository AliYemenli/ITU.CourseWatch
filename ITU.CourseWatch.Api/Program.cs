using ITU.CourseWatch.Api.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.SetDb();
builder.AddWorkers();
builder.SetSerilog();

var app = builder.Build();

app.MapEndpoints();
await app.Services.InitializeDbAsync();

await app.RunAsync();
