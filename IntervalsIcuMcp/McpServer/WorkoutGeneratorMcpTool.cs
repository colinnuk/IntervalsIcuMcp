using System.ComponentModel;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.Models;
using ModelContextProtocol.Server;

namespace IntervalsIcuMcp.McpServer;

[McpServerToolType]
public class WorkoutGeneratorMcpTool(IWorkoutGeneratorService service, IIntervalsIcuWorkoutTextService workoutTextService)
{
    private readonly IWorkoutGeneratorService _service = service;
    private readonly IIntervalsIcuWorkoutTextService _workoutTextService = workoutTextService;

    [McpServerTool]
    [Description("Generates a workout for a given sport, using HR zones, power zones, or RPE. Specify sport, title, description, and a list of intervals. Estimates TSS using the athlete profile from Intervals.icu. You do not need to get the athlete profile yourself before using this function.")]
    public async Task<Workout> GenerateWorkoutAsync(
        [Description("The sport type for the workout (e.g., Ride, Run, Swim)")] SportType sport,
        [Description("The title of the workout")] string title,
        [Description("A description of the workout")] string description,
        [Description("List of intervals for the workout")] List<WorkoutInterval> intervals)
    {
        return await _service.GenerateWorkout(sport, title, description, intervals);
    }

    [McpServerTool]
    [Description("Converts a Workout into intervals.icu workout builder text format. This tool MUST be called to display the formatted workout text that the user will copy and paste into the intervals.icu website.")]
    public async Task<string> ConvertWorkoutToIntervalsIcuTextAsync(
        [Description("The Workout to convert (typically the result from GenerateWorkoutAsync)")] Workout workout)
    {
        return await _workoutTextService.ToIntervalsIcuTextAsync(workout);
    }
}
