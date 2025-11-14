using System.Text.Json.Serialization;

namespace IntervalsIcuMcp.Models.IntervalsIcu;

public record Activity(
 [property: JsonPropertyName("id")] string Id,
 [property: JsonPropertyName("start_date_local")] DateTime StartDateLocal,
 [property: JsonPropertyName("name")] string Name,
 [property: JsonPropertyName("type")] SportType Type,
 [property: JsonPropertyName("duration")] int? Duration,
 [property: JsonPropertyName("distance")] double? Distance,
 [property: JsonPropertyName("avg_heart_rate")] double? AvgHeartRate,
 [property: JsonPropertyName("max_heart_rate")] double? MaxHeartRate,
 [property: JsonPropertyName("avg_power")] double? AvgPower,
 [property: JsonPropertyName("max_power")] double? MaxPower,
 [property: JsonPropertyName("calories")] int? Calories,
 [property: JsonPropertyName("description")] string Description,
 [property: JsonPropertyName("manual")] bool Manual,
 [property: JsonPropertyName("trainer")] bool? Trainer,
 [property: JsonPropertyName("commute")] bool Commute,
 [property: JsonPropertyName("race")] bool Race,
 [property: JsonPropertyName("moving_time")] int? MovingTime,
 [property: JsonPropertyName("total_elevation_gain")] double? TotalElevationGain,
 [property: JsonPropertyName("total_elevation_loss")] double? TotalElevationLoss,
 [property: JsonPropertyName("average_cadence")] double? AverageCadence,
 [property: JsonPropertyName("perceived_exertion")] double? PerceivedExertion,
 [property: JsonPropertyName("kg_lifted")] double? KgLifted,
 [property: JsonPropertyName("power_load")] int? PowerLoad,
 [property: JsonPropertyName("hr_load")] int? HrLoad,
 [property: JsonPropertyName("pace_load")] int? PaceLoad
);
