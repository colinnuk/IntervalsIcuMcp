using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Services;
using IntervalsIcuMcp.Models.IntervalsIcu;
using Microsoft.AspNetCore.Mvc;

namespace IntervalsIcuMcp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class McpToolController(
        IAthleteProfileRetriever athleteCache,
        IIntervalsIcuService icuService,
        IWorkoutGeneratorService workoutService) : ControllerBase
    {
        private readonly IAthleteProfileRetriever _athleteCache = athleteCache;
        private readonly IIntervalsIcuService _icuService = icuService;
        private readonly IWorkoutGeneratorService _workoutService = workoutService;

        [HttpGet("athlete-profile")]
        public async Task<ActionResult<AthleteProfile?>> GetAthleteProfile()
        {
            var result = await _athleteCache.GetAsync();
            return Ok(result);
        }

        [HttpGet("recent-activities")]
        public async Task<ActionResult<Activity[]?>> GetRecentActivities([FromQuery] int daysBehind = 42)
        {
            var result = await _icuService.GetRecentActivitiesAsync(daysBehind);
            return Ok(result);
        }

        [HttpGet("wellness")]
        public async Task<ActionResult<Wellness?>> GetWellness([FromQuery] string date)
        {
            var result = await _icuService.GetWellnessAsync(date);
            return Ok(result);
        }

        [HttpGet("upcoming-events")]
        public async Task<ActionResult<CalendarActivity[]?>> GetUpcomingEvents([FromQuery] int daysAhead = 365)
        {
            var result = await _icuService.GetFutureEventsAsync(daysAhead);
            return Ok(result);
        }

        [HttpPost("generate-workout")]
        public async Task<ActionResult<Workout>> GenerateWorkout([FromBody] GenerateWorkoutRequest request)
        {
            var result = await _workoutService.GenerateWorkout(request.Sport, request.Title, request.Description, request.Intervals);
            return Ok(result);
        }
    }
}
