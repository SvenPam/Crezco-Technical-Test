using Crezco.Application.Shared;
using Crezco.Application.Shared.Behaviours;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Crezco.Shared.Locations;
using FluentValidation;
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
    public record Query(string IpAddress) : IRequest<Response<Location>>, IIsCacheableRequest
    {
        /// <inheritdoc />
        public string CacheKey => this.IpAddress;
    }

    /// <summary>
    ///     Handler for <see cref="Query" />
    /// </summary>
    internal class Handler : IRequestHandler<Query, Response<Location>>
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
        public async Task<Response<Location>> Handle(Query request, CancellationToken cancellationToken)
        {
            // We can encapsulate query logic through CQRS and MediatR. Validation is done via
            // fluent validation. This means the functionality for Caching & Validation can be
            // in isolation, and reused for other queries or commands through interfaces.

            // Here, the class is concerned only with querying for locations.
            // Caching the query response is done via IIsCacheableRequest.
            var policy = Policy
                .HandleResult((Location?)null)
                .FallbackAsync(async cancellation =>
                {
                    // This is the main failure point. Should the external service
                    // not be available, there is not much which can be done.
                    // Retries are built into the client, along with a circuit breaker to fail
                    // fast. Practically, we would rely on SLA's for a paid service with with
                    // little downtime is mitigated by persistence and caching.
                    var location = await this.GetLocationFromExternal(request, cancellation);
                    if (location is null) return location;

                    // Arguably, this violates SRP. Given more time, this could
                    // be deferred by an event, and handled as a  background operation
                    // or function.
                    await this.AddToPersistence(cancellationToken, location);

                    return location;
                });

            return new Response<Location>
            {
                Status = Status.Success,
                Result = await policy.ExecuteAsync(() => this._locationRepository.FindLocation(request.IpAddress))
            };
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

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            this.RuleFor(x => x.IpAddress)
                .NotEmpty()
                .MinimumLength(7)
                .MaximumLength(39)
                .Must(this.BeValidIpAddress).WithMessage("IP address must be in a valid IPV4 format.");
        }

        private bool BeValidIpAddress(string ipAddress)
        {
            var ipSplitByDot = ipAddress.Split('.');
            return ipSplitByDot.Length == 4;
        }
    }
}