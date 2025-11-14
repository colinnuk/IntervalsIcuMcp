using System.ComponentModel;
using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Services;

namespace IntervalsIcuMcp.LlmPlugins;

public class WorkoutGeneratorPlugin(WorkoutGeneratorService service)
{
    private readonly WorkoutGeneratorService _service = service;

    [Description("Generates a workout for a given sport, using HR zones, power zones, or RPE. Specify sport, title, description, and a list of intervals. Estimates TSS using the athlete profile from Intervals.icu.")]
    public async Task<Workout> GenerateWorkout(
        SportType sport,
        string title,
        string description,
        List<WorkoutInterval> intervals)
    {
        return await _service.GenerateWorkout(sport, title, description, intervals);
    }
}
