using System.ComponentModel;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.Models;
using ModelContextProtocol.Server;

namespace IntervalsIcuMcp.McpServer;

[McpServerToolType]
public class WorkoutGeneratorMcpTool
{
    private readonly IWorkoutGeneratorService _service;

    public WorkoutGeneratorMcpTool(IWorkoutGeneratorService service)
    {
        _service = service;
    }

    [McpServerTool]
    [Description("Generates a workout for a given sport, using HR zones, power zones, or RPE. Specify sport, title, description, and a list of intervals. Estimates TSS using the athlete profile from Intervals.icu.")]
    public async Task<Workout> GenerateWorkoutAsync(
        [Description("The sport type for the workout (e.g., Ride, Run, Swim)")] SportType sport,
        [Description("The title of the workout")] string title,
        [Description("A description of the workout")] string description,
        [Description("List of intervals for the workout")] List<WorkoutInterval> intervals)
    {
        return await _service.GenerateWorkout(sport, title, description, intervals);
    }
}
