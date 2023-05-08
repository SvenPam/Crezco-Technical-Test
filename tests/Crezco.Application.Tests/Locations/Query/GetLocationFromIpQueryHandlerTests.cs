using Crezco.Application.Locations.Query;
using Crezco.Application.Shared;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Crezco.Shared.Locations;
using FluentAssertions;
using IPApi.Client.Locations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Crezco.Application.Tests.Locations.Query;

public class GetLocationFromIpQueryHandlerTests
{
    private readonly Mock<ILocationByIpClientService> _mockLocationByIpClientService;
    private readonly Mock<ILocationRepository> _mockLocationRepository;

    public GetLocationFromIpQueryHandlerTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        this._mockLocationByIpClientService = mockRepository.Create<ILocationByIpClientService>();
        this._mockLocationRepository = mockRepository.Create<ILocationRepository>();
    }

    private GetLocationFromIp.Handler CreateGetLocationFromIpQueryHandler() =>
        new(this._mockLocationRepository.Object, this._mockLocationByIpClientService.Object,
            NullLogger<GetLocationFromIp.Handler>.Instance);

    [Fact]
    public async Task Handle_GivenRequestAlreadyStored_ReturnsStoredLocation()
    {
        // Arrange
        var getLocationFromIpQueryHandler = this.CreateGetLocationFromIpQueryHandler();
        var request = new GetLocationFromIp.Query("24.48.0.1");
        CancellationToken cancellationToken = default;

        var expectedLocation = new Location();

        this._mockLocationRepository.Setup(x => x.FindLocation(request.IpAddress))
            .ReturnsAsync(expectedLocation);

        // Act
        var response = await getLocationFromIpQueryHandler.Handle(
            request,
            cancellationToken);

        // Assert
        response.Result.Should().Be(expectedLocation);
    }

    [Fact]
    public async Task Handle_GivenIpNotAlreadyKnown_ReturnsExternalLocation()
    {
        // Arrange
        var getLocationFromIpQueryHandler = this.CreateGetLocationFromIpQueryHandler();
        var request = new GetLocationFromIp.Query("24.48.0.1");
        CancellationToken cancellationToken = default;

        var expectedLocation = new Location
        {
            IpAddress = request.IpAddress,
            Country = "Canada",
            CountryCode = "CA",
            Region = "QC",
            RegionName = "Quebec",
            City = "Montreal",
            Zip = "H2L",
            Longitude = 45.5212f,
            Latitude = -73.5524f,
            Timezone = "America/Toronto"
        };

        this._mockLocationRepository.Setup(x => x.FindLocation(request.IpAddress))
            .ReturnsAsync((Location?)null);

        this._mockLocationByIpClientService
            .Setup(x => x.GetLocationForIp(request.IpAddress, cancellationToken))
            .ReturnsAsync(
                new LocationModel(request.IpAddress, "success", expectedLocation.Country, expectedLocation.CountryCode,
                    expectedLocation.Region, expectedLocation.RegionName, expectedLocation.City, expectedLocation.Zip,
                    expectedLocation.Latitude, expectedLocation.Longitude,
                    expectedLocation.Timezone, "", "", ""));

        this._mockLocationRepository.Setup(x => x.AddLocation(It.IsAny<Location>()));
        this._mockLocationRepository.Setup(x => x.SaveChangesAsync(cancellationToken)).ReturnsAsync(1);

        // Act
        var response = await getLocationFromIpQueryHandler.Handle(
            request,
            cancellationToken);

        // Assert
        response.Result.Should().BeEquivalentTo(expectedLocation);
    }

    [Fact]
    public async Task Handle_GivenIpNotAlreadyKnownAndNotReturnedByExternal_ReturnsNull()
    {
        // Arrange
        var getLocationFromIpQueryHandler = this.CreateGetLocationFromIpQueryHandler();
        var request = new GetLocationFromIp.Query("24.48.0.1");
        CancellationToken cancellationToken = default;

        this._mockLocationRepository.Setup(x => x.FindLocation(request.IpAddress))
            .ReturnsAsync((Location?)null);

        this._mockLocationByIpClientService
            .Setup(x => x.GetLocationForIp(request.IpAddress, cancellationToken))
            .ReturnsAsync((LocationModel?)null);

        // Act
        var response = await getLocationFromIpQueryHandler.Handle(
            request,
            cancellationToken);

        // Assert
        response.Result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_GivenIpNotAlreadyKnownAndAddToPersistenceFails_StillReturnsLocationFromExternal()
    {
        // Arrange
        var getLocationFromIpQueryHandler = this.CreateGetLocationFromIpQueryHandler();
        var request = new GetLocationFromIp.Query("24.48.0.1");
        CancellationToken cancellationToken = default;

        var expectedLocation = new Location
        {
            IpAddress = request.IpAddress,
            Country = "Canada",
            CountryCode = "CA",
            Region = "QC",
            RegionName = "Quebec",
            City = "Montreal",
            Zip = "H2L",
            Longitude = 45.5212f,
            Latitude = -73.5524f,
            Timezone = "America/Toronto"
        };

        this._mockLocationRepository.Setup(x => x.FindLocation(request.IpAddress))
            .ReturnsAsync((Location?)null);

        this._mockLocationByIpClientService
            .Setup(x => x.GetLocationForIp(request.IpAddress, cancellationToken))
            .ReturnsAsync(
                new LocationModel(request.IpAddress, "success", expectedLocation.Country, expectedLocation.CountryCode,
                    expectedLocation.Region, expectedLocation.RegionName, expectedLocation.City, expectedLocation.Zip,
                    expectedLocation.Latitude, expectedLocation.Longitude,
                    expectedLocation.Timezone, "", "", ""));

        this._mockLocationRepository.Setup(x => x.AddLocation(expectedLocation))
            .Throws<InvalidOperationException>();

        // Act
        var response = await getLocationFromIpQueryHandler.Handle(
            request,
            cancellationToken);

        // Assert
        response.Status.Should().Be(Status.Success);
        response.Result.Should().BeEquivalentTo(expectedLocation);
    }
}