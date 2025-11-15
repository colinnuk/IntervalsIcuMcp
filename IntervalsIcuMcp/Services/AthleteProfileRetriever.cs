using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.Services;

public interface IAthleteProfileRetriever
{
    Task<AthleteProfile?> GetAsync();
    void Invalidate();
}

public class AthleteProfileRetriever(
    IIntervalsIcuService intervalsIcu,
    ILogger<AthleteProfileRetriever> logger) : IAthleteProfileRetriever
{
    private readonly IIntervalsIcuService _intervalsIcu = intervalsIcu;
    private readonly ILogger<AthleteProfileRetriever> _logger = logger;
    private AthleteProfile? _cachedProfile;

    public async Task<AthleteProfile?> GetAsync()
    {
        if (_cachedProfile is not null)
        {
            return _cachedProfile;
        }
        try
        {
            _cachedProfile = await _intervalsIcu.GetAthleteProfileAsync();
            return _cachedProfile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch athlete profile from Intervals.icu");
            return null;
        }
    }

    public void Invalidate()
    {
        _cachedProfile = null;
    }
}
