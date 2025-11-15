using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.Extensions;

public static class SportTypeExtensions
{
    private static readonly SportType[] CyclingTypes =
    [
        SportType.Ride,
        SportType.VirtualRide,
        SportType.MountainBikeRide,
        SportType.GravelRide,
        SportType.EBikeRide,
        SportType.EMountainBikeRide,
        SportType.TrackRide,
        SportType.Handcycle,
        SportType.Velomobile
    ];

    private static readonly SportType[] RunningTypes =
    [
        SportType.Run,
        SportType.VirtualRun,
        SportType.TrailRun
    ];

    private static readonly SportType[] SwimmingTypes =
    [
        SportType.Swim,
        SportType.OpenWaterSwim
    ];

    private static readonly SportType[] TssSupportedTypes =
    [
        // Cycling
        SportType.Ride,
        SportType.VirtualRide,
        SportType.MountainBikeRide,
        SportType.GravelRide,
        SportType.EBikeRide,
        SportType.EMountainBikeRide,
        SportType.TrackRide,
        SportType.Handcycle,
        SportType.Velomobile,
        // Run
        SportType.Run,
        SportType.VirtualRun,
        SportType.TrailRun,
        // Swimming
        SportType.Swim,
        SportType.OpenWaterSwim,
        // Other endurance sports
        SportType.Rowing,
        SportType.VirtualRow,
        SportType.NordicSki,
        SportType.VirtualSki,
        SportType.BackcountrySki,
        SportType.RollerSki,
        SportType.Hike,
        SportType.Walk,
        SportType.Elliptical,
        SportType.Snowshoe
    ];

    public static bool IsCycling(this SportType sportType) => CyclingTypes.Contains(sportType);

    public static bool IsRunning(this SportType sportType) => RunningTypes.Contains(sportType);

    public static bool IsSwimming(this SportType sportType) => SwimmingTypes.Contains(sportType);

    public static bool SupportsTss(this SportType sportType) => TssSupportedTypes.Contains(sportType);

    public static SportSetting? GetCyclingSportSetting(this AthleteProfile profile)
    {
        return profile.SportSettings?.FirstOrDefault(s =>
            s.Types.Any(t => CyclingTypes.Contains(t)));
    }

    public static SportSetting? GetRunningSportSetting(this AthleteProfile profile)
    {
        return profile.SportSettings?.FirstOrDefault(s =>
            s.Types.Any(t => RunningTypes.Contains(t)));
    }
    public static SportSetting? GetSwimmingSportSetting(this AthleteProfile profile)
    {
        return profile.SportSettings?.FirstOrDefault(s =>
            s.Types.Any(t => SwimmingTypes.Contains(t)));
    }

    public static SportSetting? GetSportSettingByType(this AthleteProfile profile, SportType sportType)
    {
        return profile.SportSettings?.FirstOrDefault(s =>
            s.Types.Contains(sportType));
    }

    public static SportSetting GetSportSettingForProfile(this AthleteProfile profile, SportType sportType)
    {
        var settings = sportType switch
        {
            var s when s.IsCycling() => profile.GetCyclingSportSetting(),
            var s when s.IsRunning() => profile.GetRunningSportSetting(),
            var s when s.IsSwimming() => profile.GetSwimmingSportSetting(),
            _ => profile.GetSportSettingByType(sportType)
        };
        if (settings == null)
            return profile.GetSportSettingByType(SportType.Other)
                ?? throw new InvalidOperationException($"No sport settings found for {sportType} in AthleteProfile, and no 'Other' fallback available.");
        return settings;
    }
}
