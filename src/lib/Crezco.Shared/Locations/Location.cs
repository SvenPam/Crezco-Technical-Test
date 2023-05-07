namespace Crezco.Shared.Locations;

/// <summary>
///     A location somewhere in the world.
/// </summary>
public class Location
{
    public string IpAddress { get; init; } = string.Empty;

    /// <summary>Country name</summary>
    public string? Country { get; init; }

    /// <summary>Two-letter continent code</summary>
    public string? CountryCode { get; init; }

    /// <summary>Region/state short code (FIPS or ISO)</summary>
    public string? Region { get; init; }

    /// <summary>Region/state</summary>
    public string? RegionName { get; init; }

    /// <summary>City</summary>
    public string? City { get; init; }

    /// <summary>Zip code</summary>
    public string? Zip { get; init; }

    /// <summary>Latitude</summary>
    public float Latitude { get; init; }

    /// <summary>Longitude</summary>
    public float Longitude { get; init; }

    /// <summary>Timezone (tz)</summary>
    public string? Timezone { get; init; }
}