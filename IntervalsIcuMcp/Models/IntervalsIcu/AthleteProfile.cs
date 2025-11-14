using System.Text.Json.Serialization;

namespace IntervalsIcuMcp.Models.IntervalsIcu;

public record AthleteProfile(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("sex")] string? Sex,
    [property: JsonPropertyName("city")] string? City,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("country")] string? Country,
    [property: JsonPropertyName("timezone")] string Timezone,
    [property: JsonPropertyName("measurement_preference")] string? MeasurementPreference,
    [property: JsonPropertyName("icu_date_of_birth")] DateTime? IcuDateOfBirth,
    [property: JsonPropertyName("icu_resting_hr")] double? IcuRestingHr,
    [property: JsonPropertyName("icu_weight")] double? IcuWeight,
    [property: JsonPropertyName("sportSettings")] SportSetting[] SportSettings
);
