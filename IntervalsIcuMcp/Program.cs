using IntervalsIcuMcp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Bind IntervalsIcuOptions from environment variables
builder.Services.Configure<IntervalsIcuOptions>(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable(IntervalsIcuOptions.ApiKeyEnvVar) ?? string.Empty;
    options.AthleteId = Environment.GetEnvironmentVariable(IntervalsIcuOptions.AthleteIdEnvVar) ?? string.Empty;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
