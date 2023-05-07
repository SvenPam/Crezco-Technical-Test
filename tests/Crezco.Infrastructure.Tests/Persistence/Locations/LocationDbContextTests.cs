using Azure.Core;
using Crezco.Infrastructure.Persistence.Locations;
using Crezco.Shared.Locations;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Crezco.Infrastructure.Tests.Persistence.Locations;

public sealed class LocationDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public LocationDbContextTests()
    {
        this._connection = new SqliteConnection("Filename=:memory:");
        this._connection.Open();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._connection.Dispose();
    }

    private LocationDbContext CreateLocationDbContext()
    {
        var options = new DbContextOptionsBuilder<LocationDbContext>().UseSqlite(this._connection);
        var context = new LocationDbContext(options.Options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task FindLocation_WhenLocationMatchedByIp_ReturnsLocation()
    {
        // Arrange
        var locationDbContext = this.CreateLocationDbContext();
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

        locationDbContext.AddLocation(expectedLocation);
        await locationDbContext.SaveChangesAsync();

        // Act
        var result = await locationDbContext.FindLocation(
            ipAddress);

        // Assert
        result.Should().Be(expectedLocation);
    }

    [Fact]
    public async Task FindLocation_WhenLocationNotMatched_ReturnsNull()
    {
        // Arrange
        var locationDbContext = this.CreateLocationDbContext();
        // Act
        var result = await locationDbContext.FindLocation(
            "made-up");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddLocation_ProvidedLocation_AddsToLocalTransaction()
    {
        // Arrange
        var locationDbContext = this.CreateLocationDbContext();

        var expectedLocation = new Location { IpAddress = "123" };

        // Act
        locationDbContext.AddLocation(
            expectedLocation);

        // Assert
        locationDbContext.Locations.Find(expectedLocation.IpAddress).Should().Be(expectedLocation);
    }
}