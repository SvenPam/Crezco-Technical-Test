using System.Diagnostics.CodeAnalysis;
using IPApi.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Crezco.Application;

/// <summary>
///     Contains service registrations for Application services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Registration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Registration).Assembly));

        services.AddIpApiClient();

        return services;
    }
}