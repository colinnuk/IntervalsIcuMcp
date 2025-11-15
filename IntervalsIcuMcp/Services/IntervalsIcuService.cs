using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Text;
using IntervalsIcuMcp.Models.IntervalsIcu;
using IntervalsIcuMcp.Models;

namespace IntervalsIcuMcp.Services;

public interface IIntervalsIcuService
{
    Task<AthleteProfile?> GetAthleteProfileAsync();
    Task<Activity[]?> GetRecentActivitiesAsync(int daysBehind = 42);
    Task<Wellness?> GetWellnessAsync(string date);
    Task<CalendarActivity[]?> GetFutureEventsAsync(int daysAhead = 365);
    Task<CalendarActivity?> AddWorkoutEventAsync(PlannedWorkout plannedWorkout, AthleteProfile athleteProfile);
}

public class IntervalsIcuService(
    IHttpClientFactory httpClientFactory,
    IOptions<IntervalsIcuOptions> options,
    ILogger<IntervalsIcuService> logger,
    IIntervalsIcuWorkoutTextService workoutTextService) : IIntervalsIcuService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IntervalsIcuOptions _options = options.Value;
    private readonly ILogger<IntervalsIcuService> _logger = logger;
    private readonly IIntervalsIcuWorkoutTextService _workoutTextService = workoutTextService;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new();

    public async Task<AthleteProfile?> GetAthleteProfileAsync()
    {
        return await GetFromApiAsync<AthleteProfile>(
            $"athlete/{_options.AthleteId}",
            nameof(GetAthleteProfileAsync));
    }

    public async Task<Activity[]?> GetRecentActivitiesAsync(int daysBehind = 42)
    {
        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddDays(-daysBehind);

        var activities = await GetFromApiAsync<Activity[]>(
            $"athlete/{_options.AthleteId}/activities?oldest={startDate:yyyy-MM-dd}&newest={endDate:yyyy-MM-dd}",
            nameof(GetRecentActivitiesAsync));

        if (activities is not null)
        {
            _logger.LogInformation("Retrieved {ActivityCount} activities", activities.Length);
        }

        return activities;
    }

    public async Task<Wellness?> GetWellnessAsync(string date)
    {
        return await GetFromApiAsync<Wellness>(
            $"athlete/{_options.AthleteId}/wellness/{date}",
            nameof(GetWellnessAsync));
    }

    public async Task<CalendarActivity[]?> GetFutureEventsAsync(int daysAhead = 365)
    {
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(daysAhead);

        var events = await GetFromApiAsync<CalendarActivity[]>(
            $"athlete/{_options.AthleteId}/events?oldest={startDate:yyyy-MM-dd}&newest={endDate:yyyy-MM-dd}",
            nameof(GetFutureEventsAsync));

        if (events is not null)
        {
            _logger.LogInformation("Retrieved {EventCount} future events", events.Length);
        }

        return events;
    }

    public async Task<CalendarActivity?> AddWorkoutEventAsync(PlannedWorkout plannedWorkout, AthleteProfile athleteProfile)
    {
        _logger.LogInformation("Adding workout event: {WorkoutName} on {Date}", plannedWorkout.Name, plannedWorkout.DateTime);

        var workoutText = await _workoutTextService.ToIntervalsIcuTextAsync(plannedWorkout.Workout);

        // Create the event request payload
        var eventRequest = new
        {
            category = "WORKOUT",
            start_date_local = plannedWorkout.DateTime.ToString("yyyy-MM-dd"),
            name = plannedWorkout.Name,
            description = string.IsNullOrWhiteSpace(plannedWorkout.Notes)
                ? workoutText
                : $"{plannedWorkout.Notes}\n\n{workoutText}",
            type = plannedWorkout.Workout.Sport.ToString()
        };

        var result = await PostToApiAsync<CalendarActivity>(
            $"athlete/{_options.AthleteId}/events",
            eventRequest,
            nameof(AddWorkoutEventAsync));

        if (result is not null)
        {
            _logger.LogInformation("Successfully added workout event: {WorkoutName} with ID: {EventId}",
                plannedWorkout.Name, result.Id);
        }

        return result;
    }

    private async Task<T?> GetFromApiAsync<T>(string endpoint, string operationName) where T : class
    {
        _logger.LogInformation("Fetching {Operation}", operationName);

        var httpClient = _httpClientFactory.CreateClient(StringConsts.IntervalsIcuApiClientName);
        var response = await httpClient.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to {Operation}. Status: {StatusCode}", operationName, response.StatusCode);
            return null;
        }

        var jsonContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(jsonContent, _jsonSerializerOptions);

        _logger.LogInformation("Successfully retrieved {Operation}", operationName);
        return result;
    }

    private async Task<TResponse?> PostToApiAsync<TResponse>(string endpoint, object requestBody, string operationName) where TResponse : class
    {
        _logger.LogInformation("Posting {Operation}", operationName);

        var httpClient = _httpClientFactory.CreateClient(StringConsts.IntervalsIcuApiClientName);
        var jsonContent = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to {Operation}. Status: {StatusCode}, Error: {Error}",
                operationName, response.StatusCode, errorContent);
            return null;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonSerializerOptions);

        _logger.LogInformation("Successfully posted {Operation}", operationName);
        return result;
    }
}