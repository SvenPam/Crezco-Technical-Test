using Crezco.Shared.Locations;
using IPApi.Client.Locations;
using MediatR;

namespace Crezco.Application.Locations.Query;

/// <summary>
///     Query for obtaining location from IP.
/// </summary>
public static class GetLocationFromIp
{
    /// <summary>
    ///     Return a <see cref="Location" /> for a given <paramref name="IpAddress" />
    /// </summary>
    /// <param name="IpAddress">The IP to find a location for.</param>
    public record Query(string IpAddress) : IRequest<Location?>;

    /// <summary>
    ///     Handler for <see cref="Query" />
    /// </summary>
    internal class Handler : IRequestHandler<Query, Location?>
    {
        private readonly ILocationByIpClientService _locationByIpClient;

        public Handler(ILocationByIpClientService locationByIpClient)
        {
            this._locationByIpClient = locationByIpClient;
        }

        /// <inheritdoc />
        public async Task<Location?> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await this._locationByIpClient.GetLocationForIp(request.IpAddress, cancellationToken);
            return result is null
                ? null
                : new Location(result.Country, result.CountryCode, result.Region, result.RegionName,
                    result.City, result.Zip, result.Lat, result.Lon, result.Timezone);
        }
    }
}