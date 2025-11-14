namespace IntervalsIcuMcp.Models;

public class WorkoutEstimationContext
{
    public double? FtpWatts { get; init; }
    public double? LthrBpm { get; init; }
    public double? MaxHrBpm { get; init; }
    public double? RestHrBpm { get; init; }
    public int[]? PowerZones { get; init; }
    public int[]? HrZones { get; init; }
}
