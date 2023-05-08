using MediatR;
using Polly;
using Polly.Caching;

namespace Crezco.Application.Shared.Behaviours;

/// <summary>
///     Caches the
///     <typeparam name="TResponse"></typeparam>
///     for the specified
///     <typeparam name="TRequest"></typeparam>
/// </summary>
/// <typeparam name="TRequest">The request to cache.</typeparam>
/// <typeparam name="TResponse">The response to return from the cache.</typeparam>
internal class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIsCacheableRequest
{
    private readonly IAsyncCacheProvider _cacheProvider;

    public CachingBehaviour(IAsyncCacheProvider cacheProvider)
    {
        this._cacheProvider = cacheProvider;
    }

    /// <summary>
    ///     Attempts to obtain an <see cref="IIsCacheableRequest" /> from a cache, adds the
    ///     downstream result if not present.
    /// </summary>
    /// <param name="request">The request handle.</param>
    /// <param name="next">The next pipeline to handle the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The
    ///     <typeparam name="TRequest"> result.</typeparam>
    /// </returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Currently an in-memory provider is used here, however, should distributed caching be preferred
        // this can be replaced in the service registration as required, bit no change to this class.

        // A 30 min sliding TTL is used here. IIsCacheableRequest could be modified to require the
        // request define how long lived it is, and provide flexibility to the how it is used.
        var requestName = request.GetType();
        var cachePolicy = Policy.CacheAsync(this._cacheProvider, new SlidingTtl(TimeSpan.FromMinutes(30)));

        return await cachePolicy.ExecuteAsync(context => next(),
            new Context($"{requestName.Name}_{request.CacheKey}"));
    }
}