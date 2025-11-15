namespace IntervalsIcuMcp.Models;

public record Workout(
    SportType Sport,
    string Title,
    string Description,
    List<WorkoutInterval> Intervals,
    int? EstimatedTss,
    double? EstimatedIntensityFactor
);
