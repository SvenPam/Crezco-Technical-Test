using System.Net.Http.Json;

namespace IPApi.Client.Locations;

/// <inheritdoc cref="ILocationByIpClientService" />
/// />
internal sealed class LocationByIpClientService : ILocationByIpClientService
{
    private readonly HttpClient _httpClient;

    public LocationByIpClientService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }

    /// <inheritdoc />
    public void Dispose() => this._httpClient.Dispose();

    /// <inheritdoc />
    public async Task<LocationModel?> GetLocationForIp(string ipAddress, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP cannot be null or whitespace.", nameof(ipAddress));

        var response = await this._httpClient.GetAsync($"json/{ipAddress}", cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<LocationModel>(cancellationToken: cancellationToken);

        throw new IpApiRequestException(ipAddress, response.StatusCode,
            await response.Content.ReadAsStringAsync(cancellationToken));
    }
}