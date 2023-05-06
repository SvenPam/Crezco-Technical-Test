using System.Net;
using FluentAssertions;
using IPApi.Client.Locations;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace IPApi.Client.Tests.Locations;

public class LocationByIpClientServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public LocationByIpClientServiceTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);

        this._mockHttpMessageHandler = mockRepository.Create<HttpMessageHandler>();
        this._mockHttpMessageHandler.Protected()
            .Setup(
                "Dispose",
                ItExpr.IsAny<bool>()
            );
    }

    private LocationByIpClientService CreateService() =>
        new(
            new HttpClient(this._mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://ipapi.com")
            });

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetLocationForIp_WhenIpInvalidString_ThrowsArgumentException(string invalidIp)
    {
        // Arrange
        using var service = this.CreateService();
        var query = invalidIp;
        CancellationToken cancellationToken = default;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetLocationForIp(
            query,
            cancellationToken));
    }

    [Fact]
    public async Task GetLocationForIp_WhenResponseSuccessful_ReturnsLocation()
    {
        // Arrange
        using var service = this.CreateService();
        var query = "24.48.0.1";
        CancellationToken cancellationToken = default;

        this._mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new { query = "24.48.0.1" }))
            });

        // Act
        var result = await service.GetLocationForIp(
            query,
            cancellationToken);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLocationForIp_WhenResponseNotSuccessful_ThrowsIpApiRequestException()
    {
        // Arrange
        var service = this.CreateService();
        var ipAddress = "24.48.0.1";
        CancellationToken cancellationToken = default;

        var expectedCode = HttpStatusCode.InternalServerError;
        var expectedContent = "Failure";

        this._mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = expectedCode,
                Content = new StringContent(expectedContent)
            });

        // Act
        IpApiRequestException? actualException = null;
        try
        {
            await service.GetLocationForIp(
                ipAddress,
                cancellationToken);
        }
        catch (IpApiRequestException ex)
        {
            actualException = ex;
        }

        // Assert
        actualException.Should().NotBeNull();
        actualException?.RequestedIp.Should().Be(ipAddress);
        actualException?.Response.Should().Be(expectedContent);
        actualException?.StatusCode.Should().Be(expectedCode);
    }
}