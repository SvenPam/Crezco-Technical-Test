﻿using System.Diagnostics.CodeAnalysis;
using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Locations;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Crezco.Infrastructure;

/// <summary>
///     Contains service registrations for Application services.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Registration
{
    /// <summary>
    ///     Adds services required by persistence services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var cosmosConfiguration = serviceProvider.GetService<IOptions<CosmosConfiguration>>()?.Value;

        // CosmosDb is used for this application, given the Key Value nature of
        // IP/Location lookups.
        services.AddDbContext<LocationDbContext>(options =>
        {
            if (cosmosConfiguration != null)
                options.UseCosmos(
                    cosmosConfiguration.Endpoint,
                    cosmosConfiguration.Key,
                    cosmosConfiguration.DatabaseName);

            // If CosmosEmulation is not setup/working, test with SqlLite:
            // using Microsoft.Data.Sqlite;
            // var connection = new SqliteConnection("DataSource=:memory:");
            // options.UseSqlite(connection);
        });

        services.AddTransient<ILocationRepository, LocationDbContext>();
        return services;
    }

    /// <summary>
    ///     Tries to create required persistence infrastructure should it not exist.
    /// </summary>
    /// <param name="serviceScope">The current service collection.</param>
    public static void EnsurePersistenceCreated(IServiceScope serviceScope)
    {
        var dbContext = serviceScope.ServiceProvider.GetService<LocationDbContext>();
        dbContext?.Database.EnsureCreated();
    }
}