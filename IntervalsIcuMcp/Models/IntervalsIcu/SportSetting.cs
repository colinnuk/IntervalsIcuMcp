using System.Text.Json.Serialization;

namespace IntervalsIcuMcp.Models.IntervalsIcu;

public record SportSetting(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("athlete_id")] string AthleteId,
    [property: JsonPropertyName("types")] SportType[] Types,
    [property: JsonPropertyName("ftp")] int? Ftp,
    [property: JsonPropertyName("indoor_ftp")] int? IndoorFtp,
    [property: JsonPropertyName("w_prime")] int? WPrime,
    [property: JsonPropertyName("p_max")] int? PMax,
    [property: JsonPropertyName("power_zones")] int[]? PowerZones,
    [property: JsonPropertyName("power_zone_names")] string[]? PowerZoneNames,
    [property: JsonPropertyName("lthr")] int? Lthr,
    [property: JsonPropertyName("max_hr")] int? MaxHr,
    [property: JsonPropertyName("hr_zones")] int[]? HrZones,
    [property: JsonPropertyName("hr_zone_names")] string[]? HrZoneNames,
    [property: JsonPropertyName("threshold_pace")] double? ThresholdPace,
    [property: JsonPropertyName("pace_units")] string? PaceUnits
);
