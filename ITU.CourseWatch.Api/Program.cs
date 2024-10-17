using ITU.CourseWatch.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositories(builder.Configuration);
builder.AddWorkers();
builder.SetSerilog();

var app = builder.Build();

app.MapEndpoints();
await app.Services.InitializeDbAsync();

await app.RunAsync();
