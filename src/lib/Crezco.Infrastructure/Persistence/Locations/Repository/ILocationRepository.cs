using Crezco.Infrastructure.Persistence.Shared;
using Crezco.Shared.Locations;

namespace Crezco.Infrastructure.Persistence.Locations.Repository;

/// <summary>
///     Manage locations storage.
/// </summary>
public interface ILocationRepository : IUnitOfWork
{
    /// <summary>
    ///     Find a location for a given<paramref name="ipAddress" />.
    /// </summary>
    /// <param name="ipAddress">A valid IP.</param>
    /// <returns>A location when matched, <c>null</c> when not.</returns>
    public Task<Location?> FindLocation(string ipAddress);

    /// <summary>
    ///     Add a location to the current transaction.
    ///     Call <seealso cref="IUnitOfWork.SaveChangesAsync" /> to commit.
    /// </summary>
    /// <param name="location">The new location to add.</param>
    void AddLocation(Location location);
}