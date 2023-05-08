using System.Diagnostics.CodeAnalysis;
using Crezco.Application.Shared.Behaviours;
using Crezco.Infrastructure;
using FluentValidation;
using IPApi.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Polly.Caching;
using Polly.Caching.Memory;

namespace Crezco.Application;

/// <summary>
///     Contains service registrations for Application services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Registration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Registration).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingBehaviour<,>));
        });

        services.AddIpApiClient();
        services.AddPersistenceServices();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
        services.AddSingleton<IAsyncCacheProvider>(sp => memoryCacheProvider);

        services.AddValidatorsFromAssembly(typeof(Registration).Assembly);


        return services;
    }
}