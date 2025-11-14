using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Helpers;
using IntervalsIcuMcp;
using Microsoft.Extensions.Options;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.LlmPlugins;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<IntervalsIcuOptions>(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable(IntervalsIcuOptions.ApiKeyEnvVar) ?? string.Empty;
    options.AthleteId = Environment.GetEnvironmentVariable(IntervalsIcuOptions.AthleteIdEnvVar) ?? string.Empty;
});

builder.Services.AddHttpClient(StringConsts.IntervalsIcuApiClientName, (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<IntervalsIcuOptions>>().Value;
    client.ConfigureForIntervalsApi(options.ApiKey);
});

// Register application services
builder.Services.AddScoped<IIntervalsIcuService, IntervalsIcuService>();
builder.Services.AddScoped<IAthleteProfileCache, AthleteProfileCache>();
builder.Services.AddScoped<IWorkoutGeneratorService, WorkoutGeneratorService>();
builder.Services.AddScoped<IIntervalsIcuWorkoutTextService, IntervalsIcuWorkoutTextService>();
builder.Services.AddScoped<IWorkoutTssCalculator, WorkoutTssCalculator>();
builder.Services.AddScoped<IntervalsIcuPlugin>();
builder.Services.AddScoped<WorkoutGeneratorPlugin>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.Run();
