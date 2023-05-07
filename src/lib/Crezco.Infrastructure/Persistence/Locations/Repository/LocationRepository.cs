using Crezco.Shared.Locations;
using Polly;
using Polly.Caching;

namespace Crezco.Infrastructure.Persistence.Locations.Repository;

internal class LocationRepository : ILocationRepository
{
    private readonly IAsyncCacheProvider _cacheProvider;
    private readonly LocationDbContext _dbContext;

    public LocationRepository(LocationDbContext dbContext, IAsyncCacheProvider cacheProvider)
    {
        this._dbContext = dbContext;
        this._cacheProvider = cacheProvider;
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this._dbContext.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<Location?> FindLocation(string ipAddress)
    {
        var cachePolicy = Policy.CacheAsync(this._cacheProvider, new SlidingTtl(TimeSpan.FromMinutes(30)));

        return await cachePolicy.ExecuteAsync(context => this._dbContext.Locations.FindAsync(ipAddress).AsTask(),
            new Context(ipAddress));
    }

    /// <inheritdoc />
    public void AddLocation(Location location)
    {
        this._dbContext.Locations.Add(location);
    }
}