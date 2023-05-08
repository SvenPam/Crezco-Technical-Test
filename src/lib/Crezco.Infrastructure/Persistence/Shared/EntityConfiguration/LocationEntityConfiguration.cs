using Crezco.Shared.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crezco.Infrastructure.Persistence.Shared.EntityConfiguration;

internal class LocationEntityConfiguration : IEntityTypeConfiguration<Location>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(x => x.IpAddress);
        builder.HasNoDiscriminator();
        builder.HasPartitionKey(x => x.IpAddress);
        builder.ToContainer("Location");
    }
}