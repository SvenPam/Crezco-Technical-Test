namespace Crezco.Shared.Locations;

/// <summary>
///     A location somewhere in the world.
/// </summary>
/// <param name="Country">Country name</param>
/// <param name="CountryCode">Two-letter continent code</param>
/// <param name="Region">Region/state short code (FIPS or ISO)</param>
/// <param name="RegionName">Region/state</param>
/// <param name="City">City</param>
/// <param name="Zip">Zip code</param>
/// <param name="Latitude">Latitude</param>
/// <param name="Longitude">Longitude</param>
/// <param name="Timezone">Timezone (tz)</param>
public record Location(
    string Country,
    string CountryCode,
    string Region,
    string RegionName,
    string City,
    string Zip,
    float Latitude,
    float Longitude,
    string Timezone);