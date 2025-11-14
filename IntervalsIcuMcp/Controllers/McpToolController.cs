using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.LlmPlugins;
using IntervalsIcuMcp.Models.IntervalsIcu;
using Microsoft.AspNetCore.Mvc;

namespace IntervalsIcuMcp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class McpToolController(IntervalsIcuPlugin plugin, WorkoutGeneratorPlugin workoutPlugin) : ControllerBase
    {
        private readonly IntervalsIcuPlugin _plugin = plugin;
        private readonly WorkoutGeneratorPlugin _workoutPlugin = workoutPlugin;

        [HttpGet("athlete-profile")]
        public async Task<ActionResult<AthleteProfile?>> GetAthleteProfile()
        {
            var result = await _plugin.GetAthleteProfileAsync();
            return Ok(result);
        }

        [HttpGet("recent-activities")]
        public async Task<ActionResult<Activity[]?>> GetRecentActivities()
        {
            var result = await _plugin.GetRecentActivitiesAsync();
            return Ok(result);
        }

        [HttpGet("wellness")]
        public async Task<ActionResult<Wellness?>> GetWellness([FromQuery] string date)
        {
            var result = await _plugin.GetWellnessAsync(date);
            return Ok(result);
        }

        [HttpGet("upcoming-events")]
        public async Task<ActionResult<CalendarActivity[]?>> GetUpcomingEvents([FromQuery] int daysAhead = 365)
        {
            var result = await _plugin.GetUpcomingEventsAsync(daysAhead);
            return Ok(result);
        }

        [HttpPost("generate-workout")]
        public async Task<ActionResult<Workout>> GenerateWorkout([FromBody] GenerateWorkoutRequest request)
        {
            var result = await _workoutPlugin.GenerateWorkout(request.Sport, request.Title, request.Description, request.Intervals);
            return Ok(result);
        }
    }
}
