namespace IntervalsIcuMcp.Models;

public record WorkoutInterval(
    string Description,
    int DurationSeconds,
    WorkoutZoneType Type
);
