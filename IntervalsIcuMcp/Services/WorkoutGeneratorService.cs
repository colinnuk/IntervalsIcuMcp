using IntervalsIcuMcp.Extensions;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.Services;

public interface IWorkoutGeneratorService
{
   Task<Workout> GenerateWorkout(SportType sport, string title, string description, List<WorkoutInterval> intervals);
}

public class WorkoutGeneratorService(IWorkoutTssCalculator tssCalculator, IAthleteProfileRetriever athleteProfileRetriever) : IWorkoutGeneratorService
{
    private readonly IWorkoutTssCalculator _tssCalculator = tssCalculator;
    private readonly IAthleteProfileRetriever _athleteProfileRetriever = athleteProfileRetriever;

    public async Task<Workout> GenerateWorkout(
        SportType sport,
        string title,
        string description,
        List<WorkoutInterval> intervals)
    {
        var profile = await _athleteProfileRetriever.GetAsync() ?? throw new Exception("Could not retrieve profile from IntervalsIcu");
        var context = CreateContextFromProfile(profile, sport);
        var estimatedTss = _tssCalculator.EstimateTss(intervals, context, sport);
        var estimatedIf = _tssCalculator.EstimateIntensityFactor(intervals, context, sport);
        return new Workout(sport, title, description, intervals, estimatedTss, estimatedIf);
    }

    private static WorkoutEstimationContext CreateContextFromProfile(AthleteProfile profile, SportType sportType)
    {
        var sportSetting = profile.GetSportSettingForProfile(sportType);
        ValidateProfileDataForSport(sportSetting, sportType);

        return new WorkoutEstimationContext
        {
            FtpWatts = sportSetting.Ftp,
            LthrBpm = sportSetting.Lthr ?? 0,
            MaxHrBpm = sportSetting.MaxHr ?? 0,
            RestHrBpm = profile.IcuRestingHr.HasValue ? (int)profile.IcuRestingHr.Value : 0,
            PowerZones = ConvertPowerZonesToWatts(sportSetting.PowerZones, sportSetting.Ftp),
            HrZones = sportSetting.HrZones != null ? sportSetting.HrZones.ToList() : []
        };
    }

    private static List<int>? ConvertPowerZonesToWatts(int[]? powerZonesPercent, double? ftpWatts)
    {
        if (powerZonesPercent == null || !ftpWatts.HasValue)
            return null;
        int ftp = (int)Math.Round(ftpWatts.Value);
        return powerZonesPercent.Select(pz => ConvertPowerZoneFromPercentageOfFTPToWatts(pz, ftp)).ToList();
    }

    private static int ConvertPowerZoneFromPercentageOfFTPToWatts(int powerZone, int ftpWatts)
    {
        return (int)Math.Round((powerZone / 100.0) * ftpWatts);
    }

    private static void ValidateProfileDataForSport(SportSetting sportSetting, SportType sportType)
    {
        bool hasHrData = sportSetting.Lthr.HasValue && sportSetting.MaxHr.HasValue && sportSetting.HrZones != null && sportSetting.HrZones.Length > 0;
        bool hasPowerData = sportSetting.Ftp.HasValue && sportSetting.PowerZones != null && sportSetting.PowerZones.Length > 0;
        if (!hasHrData && !hasPowerData)
            throw new InvalidOperationException($"Missing HR or Power data for {sportType} in AthleteProfile.");
    }
}
