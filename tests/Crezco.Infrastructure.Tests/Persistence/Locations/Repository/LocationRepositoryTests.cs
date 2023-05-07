using Crezco.Infrastructure.Persistence.Locations;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Crezco.Shared.Locations;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Polly.Caching;

namespace Crezco.Infrastructure.Tests.Persistence.Locations.Repository;

public sealed class LocationRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly LocationDbContext _context;
    private readonly Mock<IAsyncCacheProvider> _mockAsyncProvider;

    public LocationRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<LocationDbContext>().UseSqlite(_connection);
        _context = new LocationDbContext(options.Options);
        _context.Database.EnsureCreated();

        this._mockAsyncProvider = new Mock<IAsyncCacheProvider>();

    }

    /// <inheritdoc />
    public void Dispose()
    {
        _connection.Dispose();
    }

    private LocationRepository CreateLocationRepository()
    {
        return new LocationRepository(_context, this._mockAsyncProvider.Object);
    }

    [Fact]
    public async Task FindLocation_WhenLocationMatchedByIp_ReturnsLocation()
    {
        // Arrange
        var locationRepository = CreateLocationRepository();
        var ipAddress = "123.456.789.00";

        var expectedLocation = new Location
        {
            IpAddress = ipAddress,
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

        locationRepository.AddLocation(expectedLocation);
        await locationRepository.SaveChangesAsync();

        // Act
        var result = await locationRepository.FindLocation(
            ipAddress);

        // Assert
        result.Should().Be(expectedLocation);
    }

    [Fact]
    public async Task FindLocation_WhenLocationNotMatched_ReturnsNull()
    {
        // Arrange
        var locationRepository = CreateLocationRepository();
        // Act
        var result = await locationRepository.FindLocation(
            "made-up");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddLocation_ProvidedLocation_AddsToLocalTransaction()
    {
        // Arrange
        var locationRepository = CreateLocationRepository();

        var expectedLocation = new Location { IpAddress = "123" };

        // Act
        locationRepository.AddLocation(
            expectedLocation);

        // Assert
        _context.Locations.Find(expectedLocation.IpAddress).Should().Be(expectedLocation);
    }
}