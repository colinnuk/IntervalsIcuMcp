using IntervalsIcuMcp;
using IntervalsIcuMcp.Helpers;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Services;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add local settings file (gitignored) for development secrets
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<IntervalsIcuOptions>(options =>
{
    // First, bind from configuration (appsettings.json)
    builder.Configuration.GetSection(IntervalsIcuOptions.SectionName).Bind(options);
    
    // Then override with environment variables if they exist (takes precedence)
    var apiKeyFromEnv = Environment.GetEnvironmentVariable(IntervalsIcuOptions.ApiKeyEnvVar);
    var athleteIdFromEnv = Environment.GetEnvironmentVariable(IntervalsIcuOptions.AthleteIdEnvVar);
    
    if (!string.IsNullOrEmpty(apiKeyFromEnv))
        options.ApiKey = apiKeyFromEnv;
    
    if (!string.IsNullOrEmpty(athleteIdFromEnv))
        options.AthleteId = athleteIdFromEnv;
});

builder.Services.AddHttpClient(StringConsts.IntervalsIcuApiClientName, (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<IntervalsIcuOptions>>().Value;
    client.ConfigureForIntervalsApi(options.ApiKey);
});

// Register application services
builder.Services.AddScoped<IIntervalsIcuService, IntervalsIcuService>();
builder.Services.AddScoped<IAthleteProfileRetriever, AthleteProfileRetriever>();
builder.Services.AddScoped<IWorkoutGeneratorService, WorkoutGeneratorService>();
builder.Services.AddScoped<IIntervalsIcuWorkoutTextService, IntervalsIcuWorkoutTextService>();
builder.Services.AddScoped<IWorkoutTssCalculator, WorkoutTssCalculator>();

// Configure MCP Server with HTTP transport
builder.Services.AddMcpServer()
    .WithHttpTransport() // Enable streamable HTTP for MCP
    .WithToolsFromAssembly() // Add all classes marked with [McpServerToolType]
    .WithPromptsFromAssembly(); // Add all classes marked with [McpServerPromptType]

// Configure CORS for MCP server (required for HTTP transport with GitHub Copilot)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.MapMcp("/api/mcp");
app.UseCors();

app.Run();
