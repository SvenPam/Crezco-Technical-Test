namespace Crezco.Application.Shared.Behaviours;

/// <summary>
///     This request will be cached.
/// </summary>
public interface IIsCacheableRequest
{
    public string CacheKey { get; }
}