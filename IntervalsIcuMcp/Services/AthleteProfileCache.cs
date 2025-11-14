using IntervalsIcuMcp.Models;
using IntervalsIcuMcp.Models.IntervalsIcu;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace IntervalsIcuMcp.Services;

public interface IAthleteProfileCache
{
    Task<AthleteProfile?> GetAsync(bool forceRefresh = false);
    void Invalidate();
}

public class AthleteProfileCache(
    IMemoryCache cache,
    IntervalsIcuService intervalsIcu,
    IOptions<IntervalsIcuOptions> options,
    ILogger<AthleteProfileCache> logger) : IAthleteProfileCache
{
    private readonly IMemoryCache _cache = cache;
    private readonly IntervalsIcuService _intervalsIcu = intervalsIcu;
    private readonly IOptions<IntervalsIcuOptions> _options = options;
    private readonly ILogger<AthleteProfileCache> _logger = logger;

    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(15);

    private string CacheKey => $"IntervalsIcu:Athlete:{_options.Value.AthleteId}:Profile";

    public async Task<AthleteProfile?> GetAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cache.TryGetValue<AthleteProfile>(CacheKey, out var cached))
        {
            return cached;
        }

        try
        {
            var profile = await _intervalsIcu.GetAthleteProfileAsync();
            if (profile is not null)
            {
                _cache.Set(CacheKey, profile, DefaultTtl);
            }
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh athlete profile from Intervals.icu");
            // If we have a stale cached value, return it
            if (_cache.TryGetValue<AthleteProfile>(CacheKey, out var stale))
            {
                return stale;
            }
            return null;
        }
    }

    public void Invalidate()
    {
        _cache.Remove(CacheKey);
    }
}
