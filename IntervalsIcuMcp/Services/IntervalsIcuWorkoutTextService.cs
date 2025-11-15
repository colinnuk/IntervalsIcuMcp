using IntervalsIcuMcp.Extensions;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;
using System.Text;

namespace IntervalsIcuMcp.Services;

public interface IIntervalsIcuWorkoutTextService
{
    /// <summary>
    /// Converts a <see cref="GenerateWorkoutRequest"/> into intervals.icu workout builder text.
    /// Automatically retrieves the athlete profile to convert zone targets to BPM/Watts.
    /// </summary>
    /// <param name="workout">The workout to convert.</param>
    /// <returns>The intervals.icu workout builder text.</returns>
    Task<string> ToIntervalsIcuTextAsync(GenerateWorkoutRequest workout);
}

public class IntervalsIcuWorkoutTextService(IAthleteProfileRetriever athleteProfileRetriever) : IIntervalsIcuWorkoutTextService
{
    private readonly IAthleteProfileRetriever _athleteProfileRetriever = athleteProfileRetriever;

    public async Task<string> ToIntervalsIcuTextAsync(GenerateWorkoutRequest workout)
    {
        var athleteProfile = await _athleteProfileRetriever.GetAsync() 
            ?? throw new InvalidOperationException("Could not retrieve athlete profile from Intervals.icu.");
        return ToIntervalsIcuText(workout, athleteProfile);
    }

    private static string ToIntervalsIcuText(GenerateWorkoutRequest workout, AthleteProfile athleteProfile)
    {
        var sb = new StringBuilder();

        foreach (var interval in workout.Intervals)
        {
            var line = BuildIntervalLine(interval, athleteProfile, workout.Sport);
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    private static string BuildIntervalLine(WorkoutInterval interval, AthleteProfile profile, SportType sport)
    {
        var duration = FormatDuration(interval.DurationSeconds);

        if (sport.IsCycling())
        {
            // Use power zones for cycling, with fallback to HR zones if not available
            var powerRange = GetPowerRange(interval.Type, profile, sport);
            return $"- {duration} @ {powerRange}";
        }
        else
        {
            // Use heart rate zones for other sports
            var hrRange = GetHeartRateRange(interval.Type, profile, sport);
            return $"- {duration} @ {hrRange}";
        }
    }

    private static string GetPowerRange(WorkoutZoneType zoneType, AthleteProfile profile, SportType sport)
    {
        // Get power zones from the cycling sport setting with fallback to Other
        var cyclingSport = profile.GetSportSettingForProfile(sport);
        var powerZones = cyclingSport?.PowerZones;

        // Zone index (Z1 = 0, Z2 = 1, etc.)
        int zoneIndex = (int)zoneType;

        if (powerZones is not null && powerZones.Length > zoneIndex)
        {
            // PowerZones array contains the upper boundaries of each zone as a % of FTP
            // Zone boundaries: [0 to powerZones[0]), [powerZones[0] to powerZones[1]), etc.
            int min = zoneIndex > 0 ? powerZones[zoneIndex - 1] : 40; // Set a minimum power of 40% for Z1
            int max = powerZones[zoneIndex];
            return $"{min}-{max}%";
        }
        
        // Fallback to HR zones if power zones are not available
        return GetHeartRateRange(zoneType, profile, sport);
    }

    private static string GetHeartRateRange(WorkoutZoneType zoneType, AthleteProfile profile, SportType sport)
    {
        var sportSetting = profile.GetSportSettingForProfile(sport);
        var hrZones = sportSetting?.HrZones;

        // Zone index (Z1 = 0, Z2 = 1, etc.)
        int zoneIndex = (int)zoneType;

        if (hrZones is not null && hrZones.Length > zoneIndex)
        {
            // HrZones array contains the upper boundaries of each zone
            // Zone boundaries: [0 to hrZones[0]), [hrZones[0] to hrZones[1]), etc.
            int minBpm = zoneIndex > 0 ? hrZones[zoneIndex - 1] : 100; // Set a minimum HR of 100bpm for Z1
            int maxBpm = hrZones[zoneIndex];
            return $"{minBpm}-{maxBpm}bpm";
        }
        return "Could not determine HR range";
    }

    private static string FormatDuration(int seconds)
    {
        if (seconds <= 0) return "1s"; // guard
        var ts = TimeSpan.FromSeconds(seconds);
        var parts = new List<string>();
        if (ts.Hours > 0) parts.Add($"{ts.Hours}h");
        if (ts.Minutes > 0) parts.Add($"{ts.Minutes}m");
        if (ts.Seconds > 0 && ts.Hours == 0) parts.Add($"{ts.Seconds}s"); // keep output compact; omit seconds when hours present
        return string.Join(' ', parts.Count > 0 ? parts : ["1s"]);
    }
}
