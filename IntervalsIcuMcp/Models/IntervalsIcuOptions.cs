namespace IntervalsIcuMcp.Models;

public class IntervalsIcuOptions
{
    public const string SectionName = "IntervalsIcu";
    public const string ApiKeyEnvVar = "INTERVALS_ICU_API_KEY";
    public const string AthleteIdEnvVar = "INTERVALS_ICU_ATHLETE_ID";
    
    public string ApiKey { get; set; } = string.Empty;
    public string AthleteId { get; set; } = string.Empty;
}