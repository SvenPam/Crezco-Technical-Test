namespace IPApi.Client.Locations;

/// <summary>
///     IP API Endpoints for obtaining location for IPs.
/// </summary>
public interface ILocationByIpClientService : IDisposable
{
    /// <summary>
    ///     Return a location for a given IP.
    /// </summary>
    /// <param name="ipAddress">A valid, non-empty IP address.</param>
    /// <param name="cancellationToken">The cancellation token for this request.</param>
    /// <returns>When IP matched, a location for the provided <paramref name="ipAddress" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="ipAddress" /> cannot be null, empty or whitespace.</exception>
    /// <exception cref="IpApiRequestException">
    ///     There was an issue downstream making this request, see
    ///     <seealso cref="IpApiRequestException" /> for details.
    /// </exception>
    Task<LocationModel?> GetLocationForIp(string ipAddress, CancellationToken cancellationToken);
}