using IntervalsIcuMcp.Extensions;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.Services;

public interface IWorkoutGeneratorService
{
   Task<Workout> GenerateWorkout(SportType sport, string title, string description, List<WorkoutInterval> intervals);
}

public class WorkoutGeneratorService(IWorkoutTssCalculator tssCalculator, IAthleteProfileRetriever athleteProfileCache) : IWorkoutGeneratorService
{
    private readonly IWorkoutTssCalculator _tssCalculator = tssCalculator;
    private readonly IAthleteProfileRetriever _athleteProfileCache = athleteProfileCache;

    public async Task<Workout> GenerateWorkout(
        SportType sport,
        string title,
        string description,
        List<WorkoutInterval> intervals)
    {
        var profile = await _athleteProfileCache.GetAsync();
        var context = CreateContextFromProfile(profile);
        var estimatedTss = _tssCalculator.EstimateTss(intervals.ToArray(), context, sport);
        var estimatedIf = _tssCalculator.EstimateIntensityFactor(intervals.ToArray(), context, sport);
        return new Workout(sport, title, description, intervals.ToArray(), estimatedTss, estimatedIf);
    }

    private static WorkoutEstimationContext CreateContextFromProfile(AthleteProfile? profile)
    {
        if (profile is null)
        {
            return new WorkoutEstimationContext();
        }

        // Extract cycling/bike FTP and HR data from sport settings
        var cyclingSport = profile.GetCyclingSportSetting();

        return new WorkoutEstimationContext
        {
            FtpWatts = cyclingSport?.Ftp,
            LthrBpm = cyclingSport?.Lthr,
            MaxHrBpm = cyclingSport?.MaxHr,
            RestHrBpm = profile.IcuRestingHr,
            PowerZones = cyclingSport?.PowerZones,
            HrZones = cyclingSport?.HrZones
        };
    }
}
