using Crezco.Application.Locations.Query;
using FluentAssertions;
using IPApi.Client.Locations;
using Moq;

namespace Crezco.Application.Tests.Locations.Query;

public class GetLocationFromIpQueryHandlerTests
{
    private readonly Mock<ILocationByIpClientService> _mockLocationByIpClientService;

    public GetLocationFromIpQueryHandlerTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        this._mockLocationByIpClientService = mockRepository.Create<ILocationByIpClientService>();
    }

    private GetLocationFromIp.Handler CreateGetLocationFromIpQueryHandler() =>
        new(this._mockLocationByIpClientService.Object);

    [Fact]
    public async Task Handle_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var getLocationFromIpQueryHandler = this.CreateGetLocationFromIpQueryHandler();
        var request = new GetLocationFromIp.Query("24.48.0.1");
        CancellationToken cancellationToken = default;

        this._mockLocationByIpClientService
            .Setup(x => x.GetLocationForIp(request.IpAddress, cancellationToken))
            .ReturnsAsync(
                new LocationModel(request.IpAddress, "success", "Canada", "CA", "QC", "Quebec", "Montreal", "H2L",
                    45.5212f, -73.5524f,
                    "America/Toronto", "", "", ""));

        // Act
        var result = await getLocationFromIpQueryHandler.Handle(
            request,
            cancellationToken);

        // Assert
        result.Should().NotBeNull();
    }
}