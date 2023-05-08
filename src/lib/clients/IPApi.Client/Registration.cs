using System.Diagnostics.CodeAnalysis;
using System.Net;
using IPApi.Client.Locations;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace IPApi.Client;

/// <summary>
///     Contains service registrations for IP API Client services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Registration
{
    private const string BaseUrl = "ip-api.com";

    /// <summary>
    ///     Inject services required by IP API Client.
    /// </summary>
    /// <param name="services">The current service collection.</param>
    /// <returns></returns>
    public static IServiceCollection AddIpApiClient(this IServiceCollection services)
    {

        // Registering retries and circuit breaker against the HTTP
        // client, adding some resiliency against depending on an external
        // service.
        // Would prefer HTTPS, however, this service's (https://ip-api.com)
        // premium model is via https.
        services.AddHttpClient<ILocationByIpClientService, LocationByIpClientService>(
                client => { client.BaseAddress = new Uri($"http://{BaseUrl}/"); })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}