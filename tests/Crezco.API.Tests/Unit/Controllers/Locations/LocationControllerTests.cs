using Crezco.API.Controllers.Locations;
using Crezco.Application.Locations.Query;
using Crezco.Application.Shared;
using Crezco.Shared.Locations;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Crezco.API.Tests.Unit.Controllers.Locations;

public class LocationControllerTests
{
    private readonly Mock<IMediator> _mockMediator;

    public LocationControllerTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        this._mockMediator = mockRepository.Create<IMediator>();
    }

    private LocationController CreateLocationController() =>
        new(this._mockMediator.Object);

    [Fact]
    public async Task GetLocationForIp_WhenLocationRequestedForIp_ReturnsOkResponse()
    {
        // Arrange
        var locationController = this.CreateLocationController();
        var ipAddress = "24.48.0.1";

        var location = new Response<Location>();

        this._mockMediator
            .Setup(x => x.Send(It.IsAny<GetLocationFromIp.Query>(), default))
            .ReturnsAsync(location);

        // Act
        var result = await locationController.GetLocationForIp(
            ipAddress);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}