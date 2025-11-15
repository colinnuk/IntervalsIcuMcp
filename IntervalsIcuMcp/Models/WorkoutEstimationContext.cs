namespace IntervalsIcuMcp.Models;

public class WorkoutEstimationContext
{
    public double? FtpWatts { get; init; }
    public required int LthrBpm { get; init; }
    public required int MaxHrBpm { get; init; }
    public required int RestHrBpm { get; init; }
    public List<int>? PowerZones { get; init; }
    public required List<int> HrZones { get; init; }
}
