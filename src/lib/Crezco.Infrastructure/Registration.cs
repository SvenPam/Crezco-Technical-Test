using Crezco.Infrastructure.Persistence;
using Crezco.Infrastructure.Persistence.Locations;
using Crezco.Infrastructure.Persistence.Locations.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Polly.Caching.Memory;
using Polly.Caching;

namespace Crezco.Infrastructure
{
    /// <summary>
    ///     Contains service registrations for Application services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Registration
    {
        /// <summary>
        /// Adds services required by persistence services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var cosmosConfiguration = serviceProvider.GetService<IOptions<CosmosConfiguration>>()?.Value;
            services.AddDbContext<LocationDbContext>(options =>
            {
                if (cosmosConfiguration != null)
                    options.UseCosmos(
                        cosmosConfiguration.Endpoint,
                        cosmosConfiguration.Key,
                        cosmosConfiguration.DatabaseName);
            });

            services.AddTransient<ILocationRepository, LocationRepository>();


            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            services.AddSingleton<IAsyncCacheProvider>(sp => memoryCacheProvider);

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
}
