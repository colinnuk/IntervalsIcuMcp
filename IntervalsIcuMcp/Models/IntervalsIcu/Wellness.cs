using System.Text.Json.Serialization;

namespace IntervalsIcuMcp.Models.IntervalsIcu;

public record Wellness(
 [property: JsonPropertyName("id")] string Id,
 [property: JsonPropertyName("updated")] DateTime Updated,
 [property: JsonPropertyName("ctl")] double? Ctl,
 [property: JsonPropertyName("atl")] double? Atl,
 [property: JsonPropertyName("ctlLoad")] double? CtlLoad,
 [property: JsonPropertyName("atlLoad")] double? AtlLoad,
 [property: JsonPropertyName("weight")] double? Weight,
 [property: JsonPropertyName("restingHR")] int? RestingHR,
 [property: JsonPropertyName("sleepSecs")] int? SleepSecs,
 [property: JsonPropertyName("sleepScore")] double? SleepScore,
 [property: JsonPropertyName("sleepQuality")] int? SleepQuality
);
