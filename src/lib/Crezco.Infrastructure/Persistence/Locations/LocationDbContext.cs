﻿using Crezco.Infrastructure.Persistence.Locations.Repository;
using Crezco.Infrastructure.Persistence.Shared.EntityConfiguration;
using Crezco.Shared.Locations;
using Microsoft.EntityFrameworkCore;

namespace Crezco.Infrastructure.Persistence.Locations;

/// <summary>
///     The unit of work context for locations.
/// </summary>
internal class LocationDbContext : DbContext, ILocationRepository
{
    /// <summary>
    ///     Initialize a new <see cref="LocationDbContext" /> with the provided options.
    /// </summary>
    /// <param name="options">Options this context should use.</param>
    public LocationDbContext(DbContextOptions<LocationDbContext> options) : base(options)
    {
    }

    internal DbSet<Location> Locations => this.Set<Location>();

    /// <inheritdoc />
    public async Task<Location?> FindLocation(string ipAddress) => await this.Locations.FindAsync(ipAddress);

    /// <inheritdoc />
    public void AddLocation(Location location)
    {
        this.Locations.Add(location);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new LocationEntityConfiguration());
    }
}