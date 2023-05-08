using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Crezco.API.Tests.Integration.Locations;

public class LocationTests : IClassFixture<ApiFactory<Program>>
{
    private readonly HttpClient _client;

    public LocationTests(
        ApiFactory<Program> factory)
    {
        this._client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }


    [Fact]
    public async Task Get_LocationFromIp_ReturnsLocation()
    {
        var response = await this._client.GetAsync("/location/ip/82.11.238.59");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Get_LocationFromInvalidIp_ReturnsBadRequest()
    {
        var response = await this._client.GetAsync("/location/ip/wrong format");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}