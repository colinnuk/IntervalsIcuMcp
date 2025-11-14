using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.Extensions.AI;

namespace IntervalsIcuMcp.McpServer.Prompts;

/// <summary>
/// Provides starter prompts that guide users to provide enough context for an LLM
/// to make training recommendations for endurance sports and strength training.
/// These prompts are exposed through the MCP protocol's prompts/list and prompts/get endpoints.
/// </summary>
[McpServerPromptType]
public static class TrainingRecommendationPrompts
{
    [McpServerPrompt, Description("Analyze the athlete's current fitness status, recovery state, and readiness for training based on recent activities, wellness data, and fitness metrics")]
    public static ChatMessage AnalyzeFitnessStatus(
        [Description("Number of days to look back (default: 42)")] int daysBehind = 42)
    {
        var systemPrompt = @"You are an expert endurance sports coach with deep knowledge of periodization, training science, and athlete recovery. 
You analyze athlete data including recent training load (ATL/CTL), wellness metrics, sleep quality, resting heart rate, and activity patterns.
Provide actionable insights on the athlete's fitness trajectory, recovery status, and training readiness.
Reference specific metrics from the provided data to support your recommendations.";

        var userMessage = @$"Please analyze my training status based on the last {daysBehind} days of data:

Recent Activities:
{{recent_activities}}

Current Wellness Data:
{{wellness_data}}

Athlete Profile (FTP, Max HR, Weight):
{{athlete_profile}}

Based on this data, please provide:
1. Current fitness status assessment (aerobic/anaerobic fitness, TSS trends)
2. Recovery and readiness evaluation (sleep, HRV proxy via resting HR, fatigue level)
3. Training load distribution analysis (volume vs. intensity balance)
4. Recommendations for the next training block (7-14 days)";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Create a personalized endurance training block (cycling/running) based on athlete fitness, goals, and schedule")]
    public static ChatMessage PlanEnduranceTrainingBlock(
        [Description("Training goal (e.g., 'build aerobic base', 'prepare for 100k race')")] string goal,
        [Description("Duration in days")] int durationDays,
        [Description("Primary sport (cycling or running)")] string primarySport)
    {
        var userMessage = @$"Create a {durationDays}-day training block for my {primarySport} with this goal: {goal}

My Current Fitness Profile:
{{athlete_profile}}

Recent Training History (42 days):
{{recent_activities}}

Upcoming Calendar Events:
{{upcoming_events}}

Current Wellness Status:
{{wellness_data}}

Please provide:
1. Training block strategy (periodization approach, intensity distribution)
2. 7-10 specific workouts with:
   - Sport type and duration
   - Intensity zones (% FTP or % LTHR)
   - Expected TSS and recovery day recommendations
3. Weekly structure (easy, threshold, VO2max, etc.)
4. Recovery days and cross-training suggestions
5. Testing/benchmark workout recommendations";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Design strength and conditioning workouts to complement endurance training and address specific weaknesses")]
    public static ChatMessage StrengthTrainingRecommendations(
        [Description("Focus area (e.g., 'hip stability', 'core strength', 'upper body power')")] string focusArea)
    {
        var userMessage = @$"Design strength training recommendations to address: {focusArea}

My Profile and Current Training:
{{athlete_profile}}

Recent Training Load and Activities:
{{recent_activities}}

Current Wellness Metrics:
{{wellness_data}}

Please provide:
1. Strength training periodization strategy aligned with my endurance schedule
2. 3-4 specific sessions per week with:
   - Target muscle groups
   - Exercise selections
   - Rep ranges and intensity
   - Rest periods
3. Integration schedule (which days relative to hard endurance workouts)
4. Progression plan for 4-6 weeks
5. Injury prevention exercises and mobility work";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Create a race-specific preparation plan tapering to a target event with race-day strategy")]
    public static ChatMessage RacePreparationPlan(
        [Description("Race date in yyyy-MM-dd format")] string raceDate,
        [Description("Type of race (e.g., '100K, 50 mile ultra, century ride')")] string raceType)
    {
        var userMessage = @$"Help me prepare for a {raceType} on {raceDate}

My Fitness Profile:
{{athlete_profile}}

Recent Training (last 8 weeks):
{{recent_activities}}

Current Wellness and Fatigue Status:
{{wellness_data}}

Please create a race preparation plan including:
1. Recommended taper (duration and intensity reduction)
2. Race-specific workout recommendations (2-3 weeks out)
3. Pacing strategy based on my fitness metrics
4. Race-day nutrition and hydration plan
5. Mental preparation and contingency strategies
6. Post-race recovery recommendations";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Assess recovery status and check for signs of overtraining or excessive fatigue")]
    public static ChatMessage RecoveryAndOverttrappingAssessment(
        [Description("Assessment date in yyyy-MM-dd format")] string assessmentDate)
    {
        var userMessage = @$"Please assess my recovery status and training sustainability as of {assessmentDate}

Recent Training Load Metrics:
- ATL (Acute Training Load): {{atl}}
- CTL (Chronic Training Load): {{ctl}}

Recent Activities (7 days):
{{recent_activities}}

Wellness Data (last 7 days):
{{wellness_data}}

Athlete Profile:
{{athlete_profile}}

Please evaluate:
1. Signs of overtraining or excessive fatigue
2. Recovery capacity assessment
3. Resting heart rate and sleep trend analysis
4. Recommendations for immediate training adjustments
5. Recovery protocols to implement (active recovery, rest days, sleep optimization)
6. If concerning, recommended deload week structure";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Suggest complementary training activities (strength, other sports, mobility) to enhance primary sport performance")]
    public static ChatMessage CrossTrainingSuggestions(
        [Description("Primary sport (cycling or running)")] string primarySport,
        [Description("Available hours per week")] double availabilityHoursPerWeek)
    {
        var userMessage = @$"Suggest cross-training activities to enhance my {primarySport} performance with {availabilityHoursPerWeek} hours per week available

My Current Program:
{{recent_activities}}

Fitness Profile:
{{athlete_profile}}

Wellness Status:
{{wellness_data}}

Upcoming Calendar:
{{upcoming_events}}

Please recommend:
1. Best cross-training activities (strength, swimming, other sports, mobility work)
2. Weekly cross-training schedule (days, durations, intensity)
3. How to integrate without compromising primary sport training
4. Expected benefits for my {primarySport} performance
5. Specific exercise recommendations for weak areas";

        return new ChatMessage(ChatRole.User, userMessage);
    }

    [McpServerPrompt, Description("Create a winter base building phase emphasizing aerobic development and sustainable volume")]
    public static ChatMessage WinterBaseBuilding()
    {
        var userMessage = @"Help me plan my winter base building block

Current Fitness Snapshot:
{athlete_profile}

Recent Training Pattern:
{recent_activities}

Current Wellness:
{wellness_data}

Please create a 12-week winter base plan:
1. Training philosophy and weekly structure
2. Target zones for base building (Zone 2 emphasis, some tempo/threshold)
3. Weekly volume progression
4. Cross-training and strength work integration
5. Monthly progression milestones
6. Flexibility for weather/life adjustments
7. Transition out of base into build phase";

        return new ChatMessage(ChatRole.User, userMessage);
    }
}
