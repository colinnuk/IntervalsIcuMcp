using Microsoft.Extensions.Logging;
using IntervalsIcuMcp.Extensions;
using IntervalsIcuMcp.Models;

namespace IntervalsIcuMcp.Services;

public interface IWorkoutTssCalculator
{
    int? EstimateTss(IEnumerable<WorkoutInterval> workoutIntervals, WorkoutEstimationContext context, SportType sport);
    double? EstimateIntensityFactor(IEnumerable<WorkoutInterval> workoutIntervals, WorkoutEstimationContext context, SportType sport);
}

public class WorkoutTssCalculator(ILogger<WorkoutTssCalculator> logger) : IWorkoutTssCalculator
{
    private readonly ILogger<WorkoutTssCalculator> _logger = logger;

    /// <summary>
    /// Estimate Training Stress Score (TSS) for the workout.
    /// TSS is computed as sum over intervals: durationHours * IF^2 * 100.
    /// IF is derived from the interval zone depending on the zone type and available athlete context.
    /// Returns null for sports that don't support TSS calculation.
    /// </summary>
    public int? EstimateTss(IEnumerable<WorkoutInterval> workoutIntervals, WorkoutEstimationContext context, SportType sport)
    {
        _logger.LogInformation(
            "EstimateTss called with sport: {Sport}, intervals count: {IntervalsCount}, FtpWatts: {FtpWatts}, PowerZones: {PowerZones}, LthrBpm: {LthrBpm}, HrZones: {HrZones}",
            sport,
            workoutIntervals?.Count() ?? 0,
            context.FtpWatts,
            context.PowerZones,
            context.LthrBpm,
            context.HrZones
        );
        if (!sport.SupportsTss() || workoutIntervals is null)
        {
            return null;
        }

        double totalTss = 0;
        foreach (var interval in workoutIntervals)
        {
            var ifValue = EstimateIf(interval, context, sport);
            var durationHours = Math.Max(0, interval.DurationSeconds) / 3600d;
            _logger.LogDebug("Interval: {Interval}, IF: {IfValue}, DurationHours: {DurationHours}", interval, ifValue, durationHours);
            totalTss += durationHours * ifValue * ifValue * 100;
        }
        int result = (int)Math.Round(totalTss);
        _logger.LogInformation("Estimated TSS: {Tss}", result);
        return result;
    }

    /// <summary>
    /// Estimate the overall Intensity Factor (IF) for the workout.
    /// IF is calculated as the duration-weighted average of individual interval IFs.
    /// Returns null for sports that don't support TSS calculation.
    /// </summary>
    public double? EstimateIntensityFactor(IEnumerable<WorkoutInterval> workoutIntervals, WorkoutEstimationContext context, SportType sport)
    {
        _logger.LogInformation(
            "EstimateIntensityFactor called with sport: {Sport}, intervals count: {IntervalsCount}, FtpWatts: {FtpWatts}, PowerZones: {PowerZones}, LthrBpm: {LthrBpm}, HrZones: {HrZones}",
            sport,
            workoutIntervals?.Count() ?? 0,
            context.FtpWatts,
            context.PowerZones,
            context.LthrBpm,
            context.HrZones
        );
        if (!sport.SupportsTss() || workoutIntervals is null)
        {
            return null;
        }

        double totalWeightedIf = 0;
        double totalDuration = 0;

        foreach (var interval in workoutIntervals)
        {
            var ifValue = EstimateIf(interval, context, sport);
            var durationSeconds = Math.Max(0, interval.DurationSeconds);
            totalWeightedIf += ifValue * durationSeconds;
            totalDuration += durationSeconds;
        }

        if (totalDuration == 0)
        {
            return 0;
        }

        double result = Math.Round(totalWeightedIf / totalDuration, 3);
        return result;
    }

    private static double EstimateIf(WorkoutInterval interval, WorkoutEstimationContext ctx, SportType sport)
    {
        if (sport.IsCycling())
        {
            return EstimateIfFromPowerZone(interval.Type, ctx);
        }
        return EstimateIfFromHeartRateZone(interval.Type, ctx);
    }

    private static double EstimateIfFromPowerZone(WorkoutZoneType zoneType, WorkoutEstimationContext ctx)
    {
        int zoneIndex = (int)zoneType;

        // If we have power zones and FTP, calculate IF from the zone boundaries
        if (ctx.PowerZones is not null && ctx.PowerZones.Count > zoneIndex && ctx.FtpWatts is double ftp && ftp > 0)
        {
            // PowerZones array contains the upper boundaries of each zone
            // Calculate the midpoint of the zone as a fraction of FTP
            int minWatts = zoneIndex > 0 ? ctx.PowerZones[zoneIndex - 1] : 0;
            int maxWatts = ctx.PowerZones[zoneIndex];
            double avgWatts = (minWatts + maxWatts) / 2.0;
            double ifValue = avgWatts / ftp;
            return Clamp(ifValue, 0.4, 2.5);
        }

        return Clamp(0.5, 0.4, 2.5);
    }

    private static double EstimateIfFromHeartRateZone(WorkoutZoneType zoneType, WorkoutEstimationContext ctx)
    {
        int zoneIndex = (int)zoneType;

        // If we have HR zones and LTHR, calculate IF from the zone boundaries
        if (ctx.HrZones is not null && ctx.HrZones.Count > zoneIndex && ctx.LthrBpm is int lthr && lthr > 0)
        {
            // HrZones array contains the upper boundaries of each zone
            // Calculate the midpoint of the zone as a fraction of LTHR
            int minBpm = zoneIndex > 0 ? ctx.HrZones[zoneIndex - 1] : 0;
            int maxBpm = ctx.HrZones[zoneIndex];
            double avgBpm = (minBpm + maxBpm) / 2.0;
            double ifValue = avgBpm / lthr;
            return Clamp(ifValue, 0.4, 1.5);
        }

        return Clamp(0.5, 0.4, 1.5);
    }

    private static double Clamp(double v, double min, double max) => v < min ? min : v > max ? max : v;
}
