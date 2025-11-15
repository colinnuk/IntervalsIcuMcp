using System.ComponentModel;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.Models.IntervalsIcu;
using ModelContextProtocol.Server;

namespace IntervalsIcuMcp.McpServer;

[McpServerToolType]
public class IntervalsIcuMcpTool(IAthleteProfileRetriever athleteCache, IIntervalsIcuService icuService)
{
    private readonly IAthleteProfileRetriever _athleteCache = athleteCache;
    private readonly IIntervalsIcuService _icuService = icuService;

    [McpServerTool]
    [Description("Gets the athlete's profile information including FTP, weight, heart rate zones, and other fitness metrics (cached).")]
    public async Task<AthleteProfile?> GetAthleteProfileAsync()
    {
        return await _athleteCache.GetAsync();
    }

    [McpServerTool]
    [Description("Gets recent activities from the athlete's calendar including workouts, races, and training sessions")]
    public async Task<Activity[]?> GetRecentActivitiesAsync(
        [Description("Number of days to look back (default: 42)")] int daysBehind = 42)
    {
        return await _icuService.GetRecentActivitiesAsync(daysBehind);
    }

    [McpServerTool]
    [Description("Gets the athlete's wellness data for a specific date (yyyy-MM-dd), including fatigue, soreness, motivation, sleep quality, stress, RPE, and notes")]
    public async Task<Wellness?> GetWellnessAsync(
        [Description("The date in yyyy-MM-dd format")] string date)
    {
        return await _icuService.GetWellnessAsync(date);
    }

    [McpServerTool]
    [Description("Gets upcoming calendar events (planned workouts/races) for the next N days; default 365 days ahead")]
    public async Task<CalendarActivity[]?> GetUpcomingEventsAsync(
        [Description("Number of days to look ahead (default: 365)")] int daysAhead = 365)
    {
        return await _icuService.GetFutureEventsAsync(daysAhead);
    }
}
