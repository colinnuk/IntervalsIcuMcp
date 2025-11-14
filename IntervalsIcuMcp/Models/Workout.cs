using IntervalsIcuMcp.Models;

namespace IntervalsIcuMcp.Models;

public record Workout(
    SportType Sport,
    string Title,
    string Description,
    WorkoutInterval[] Intervals,
    int? EstimatedTss,
    double? EstimatedIntensityFactor
);
