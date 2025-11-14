namespace IntervalsIcuMcp.Models.IntervalsIcu;

public enum CalendarActivityCategory
{
    WORKOUT,
    RACE_A,
    RACE_B,
    RACE_C,
    NOTE,
    HOLIDAY,
    SICK,
    INJURED,
    SET_EFTP,
    FITNESS_DAYS,
    SEASON_START,
    TARGET,
    SET_FITNESS
}

public enum CalendarActivityTarget
{
    AUTO,
    POWER,
    HR,
    PACE
}

public record CalendarActivity(
    int Id,
    string StartDateLocal,
    int IcuTrainingLoad,
    double IcuAtl,
    double IcuCtl,
    SportType Type,
    int CalendarId,
    CalendarActivityCategory Category,
    string EndDateLocal,
    string Name,
    string Description,
    bool Indoor,
    int MovingTime,
    int IcuFtp,
    int AtlDays,
    int CtlDays,
    DateTime Updated,
    CalendarActivityTarget Target,
    double Distance,
    int LoadTarget,
    int TimeTarget,
    double IcuIntensity,
    double StrainScore
);
