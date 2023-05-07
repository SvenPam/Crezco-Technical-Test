using Crezco.Application.Shared.Behaviours;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Crezco.Shared.Locations;
using IPApi.Client.Locations;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;

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
    public record Query(string IpAddress) : IRequest<Location?>, IIsCacheableRequest
    {
        /// <inheritdoc />
        public string CacheKey => IpAddress;
    }

    /// <summary>
    ///     Handler for <see cref="Query" />
    /// </summary>
    internal class Handler : IRequestHandler<Query, Location?>
    {
        private readonly ILocationByIpClientService _locationByIpClient;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(ILocationRepository locationRepository, ILocationByIpClientService locationByIpClient,
            ILogger<Handler> logger)
        {
            this._locationRepository = locationRepository;
            this._locationByIpClient = locationByIpClient;
            this._logger = logger;
        }

        /// <inheritdoc />
        public async Task<Location?> Handle(Query request, CancellationToken cancellationToken)
        {
            var policy = Policy
                .HandleResult((Location?)null)
                .FallbackAsync(async cancellation =>
                {
                    var location = await this.GetLocationFromExternal(request, cancellation);
                    if (location is null) return location;

                    await this.AddToPersistence(cancellationToken, location);

                    return location;
                });

            return await policy.ExecuteAsync(() => this._locationRepository.FindLocation(request.IpAddress));
        }

        private async Task AddToPersistence(CancellationToken cancellationToken, Location location)
        {
            try
            {
                this._locationRepository.AddLocation(location);
                await this._locationRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Failed to save record to persistence, defer to external response.");
            }
        }

        private async Task<Location?> GetLocationFromExternal(Query request, CancellationToken cancellationToken)
        {
            var result = await this._locationByIpClient.GetLocationForIp(request.IpAddress, cancellationToken);
            var location = result is null
                ? null
                : new Location
                {
                    IpAddress = result.Query, 
                    Country = result.Country, 
                    CountryCode = result.CountryCode,  
                    Region = result.Region, 
                    RegionName = result.RegionName,
                    City = result.City,
                    Zip = result.Zip,
                    Latitude = result.Lat,
                    Longitude = result.Lon, 
                    Timezone = result.Timezone
                };
            return location;
        }
    }
}