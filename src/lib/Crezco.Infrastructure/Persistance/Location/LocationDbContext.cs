using Microsoft.EntityFrameworkCore;

namespace Crezco.Infrastructure.Persistance.Location;

internal class LocationDbContext : DbContext
{
    internal DbSet<Shared.Locations.Location> Locations => Set<Shared.Locations.Location>();
}