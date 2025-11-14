using IntervalsIcuMcp.Models.IntervalsIcu;

namespace IntervalsIcuMcp.Services;

public interface IAthleteProfileCache
{
    Task<AthleteProfile?> GetAsync();
    void Invalidate();
}

public class AthleteProfileCache(
    IntervalsIcuService intervalsIcu,
    ILogger<AthleteProfileCache> logger) : IAthleteProfileCache
{
    private readonly IntervalsIcuService _intervalsIcu = intervalsIcu;
    private readonly ILogger<AthleteProfileCache> _logger = logger;
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
