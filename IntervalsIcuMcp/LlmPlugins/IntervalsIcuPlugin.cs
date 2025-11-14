using System.ComponentModel;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.LlmPlugins;

// This class is the SK-facing abstraction layer. It exposes kernel functions and delegates to IntervalsIcuService (and caches if present).
public class IntervalsIcuPlugin(IAthleteProfileCache athleteCache, IIntervalsIcuService icuService)
{
    private readonly IAthleteProfileCache _athleteCache = athleteCache;
    private readonly IIntervalsIcuService _icuService = icuService;

    [Description("Gets the athlete's profile information including FTP, weight, heart rate zones, and other fitness metrics (cached).")]
    public async Task<AthleteProfile?> GetAthleteProfileAsync()
    {
        return await _athleteCache.GetAsync();
    }

    [Description("Gets the last 6 weeks of activities from the athlete's calendar including workouts, races, and training sessions")]
    public async Task<Activity[]?> GetRecentActivitiesAsync()
    {
        return await _icuService.GetRecentActivitiesAsync();
    }

    [Description("Gets the athlete's wellness data for a specific date (yyyy-MM-dd), including fatigue, soreness, motivation, sleep quality, stress, RPE, and notes")]
    public async Task<Wellness?> GetWellnessAsync(string date)
    {
        return await _icuService.GetWellnessAsync(date);
    }

    [Description("Gets upcoming calendar events (planned workouts/races) for the next N days; default 365 days ahead")]
    public async Task<CalendarActivity[]?> GetUpcomingEventsAsync(int daysAhead = 365)
    {
        return await _icuService.GetFutureEventsAsync(daysAhead);
    }
}
