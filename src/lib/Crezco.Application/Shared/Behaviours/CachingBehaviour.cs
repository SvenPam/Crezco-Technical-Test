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

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType();
        var cachePolicy = Policy.CacheAsync(this._cacheProvider, new SlidingTtl(TimeSpan.FromMinutes(30)));

        return await cachePolicy.ExecuteAsync(context => next(),
            new Context($"{requestName.Name}_{request.CacheKey}"));
    }
}