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

        // Here the HTTP Client is injected through the HTTP Client Factory, which allows two things:
        // 1. Good connection management
        // 2. The ability to define resilience at a global level. The registration for this HTTP
        //    client includes a retry policy.
        var response = await this._httpClient.GetAsync($"json/{ipAddress}", cancellationToken);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<LocationModel>(cancellationToken: cancellationToken);

        throw new IpApiRequestException(ipAddress, response.StatusCode,
            await response.Content.ReadAsStringAsync(cancellationToken));
    }
}