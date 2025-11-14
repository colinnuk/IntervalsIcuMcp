using IntervalsIcuMcp.Extensions;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;
using System.Text;

namespace IntervalsIcuMcp.Services;

public interface IIntervalsIcuWorkoutTextService
{
    /// <summary>
    /// Converts a <see cref="Workout"/> into intervals.icu workout builder text.
    /// Optionally provide an athlete profile to convert fractional heart rate targets to BPM.
    /// </summary>
    /// <param name="workout">The workout to convert.</param>
    /// <param name="athleteProfile">athlete profile used to map fractional heart rate targets to BPM using LTHR/MaxHR.</param>
    /// <returns>The intervals.icu workout builder text.</returns>
    string ToIntervalsIcuText(Workout workout, AthleteProfile athleteProfile);
}

public class IntervalsIcuWorkoutTextService : IIntervalsIcuWorkoutTextService
{
    public string ToIntervalsIcuText(Workout workout, AthleteProfile athleteProfile)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(workout.Title))
        {
            sb.AppendLine($"# {workout.Title}");
        }
        if (!string.IsNullOrWhiteSpace(workout.Description))
        {
            foreach (var line in workout.Description.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                sb.AppendLine($"# {line}");
            }
        }

        foreach (var interval in workout.Intervals)
        {
            var line = BuildIntervalLine(interval, athleteProfile, workout.Sport);
            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    private static string BuildIntervalLine(WorkoutInterval interval, AthleteProfile? profile, SportType sport)
    {
        var duration = FormatDuration(interval.DurationSeconds);
        var zoneName = interval.Type.ToString();

        if (sport.IsCycling())
        {
            // Use power zones for cycling
            var powerRange = GetPowerRange(interval.Type, profile);
            return AppendComment($"{duration} @ {powerRange}", $"{zoneName}{AppendDescription(interval.Description)}");
        }
        else
        {
            // Use heart rate zones for other sports
            var hrRange = GetHeartRateRange(interval.Type, profile);
            return AppendComment($"{duration} @ {hrRange}", $"{zoneName}{AppendDescription(interval.Description)}");
        }
    }

    private static string AppendDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description)) return string.Empty;
        return $" - {description}";
    }

    private static string GetPowerRange(WorkoutZoneType zoneType, AthleteProfile? profile)
    {
        // Get FTP and power zones from profile
        var cyclingSport = profile?.GetCyclingSportSetting();
        var powerZones = cyclingSport?.PowerZones;

        // Zone index (Z1 = 0, Z2 = 1, etc.)
        int zoneIndex = (int)zoneType;

        if (powerZones is not null && powerZones.Length > zoneIndex)
        {
            // PowerZones array contains the upper boundaries of each zone
            // Zone boundaries: [0 to powerZones[0]), [powerZones[0] to powerZones[1]), etc.
            int minWatts = zoneIndex > 0 ? powerZones[zoneIndex - 1] : 0;
            int maxWatts = powerZones[zoneIndex];
            return $"{minWatts}-{maxWatts}W";
        }
        return "";
    }

    private static string GetHeartRateRange(WorkoutZoneType zoneType, AthleteProfile? profile)
    {
        // Get LTHR and HR zones from profile
        var sportSetting = profile?.GetRunningSportSetting();
        var hrZones = sportSetting?.HrZones;

        // Zone index (Z1 = 0, Z2 = 1, etc.)
        int zoneIndex = (int)zoneType;

        if (hrZones is not null && hrZones.Length > zoneIndex)
        {
            // HrZones array contains the upper boundaries of each zone
            // Zone boundaries: [0 to hrZones[0]), [hrZones[0] to hrZones[1]), etc.
            int minBpm = zoneIndex > 0 ? hrZones[zoneIndex - 1] : 0;
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

    private static string AppendComment(string line, string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment)) return line;
        // collapse newlines to a single line comment
        var oneLine = comment.Replace('\r', ' ').Replace('\n', ' ').Trim();
        return $"{line} # {oneLine}";
    }
}
