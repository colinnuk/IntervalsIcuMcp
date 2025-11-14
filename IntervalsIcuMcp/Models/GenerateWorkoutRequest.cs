namespace IntervalsIcuMcp.Models;

public record GenerateWorkoutRequest(
    SportType Sport,
    string Title,
    string Description,
    List<WorkoutInterval> Intervals
);