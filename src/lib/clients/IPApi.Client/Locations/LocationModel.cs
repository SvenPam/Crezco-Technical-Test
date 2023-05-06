namespace IPApi.Client.Locations;

/// <summary>
///     Location return from IP API for a given IP.
/// </summary>
/// <param name="Query">IP used for the query</param>
/// <param name="Status"><c>success</c>c> or <c>fail</c></param>
/// <param name="Country">Country name</param>
/// <param name="CountryCode">Two-letter continent code</param>
/// <param name="Region">Region/state short code (FIPS or ISO)</param>
/// <param name="RegionName">Region/state</param>
/// <param name="City">City</param>
/// <param name="Zip">Zip code</param>
/// <param name="Lat">Latitude</param>
/// <param name="Lon">Longitude</param>
/// <param name="Timezone">Timezone (tz)</param>
/// <param name="Isp">ISP name</param>
/// <param name="Org">Organization name</param>
/// <param name="As">
///     AS number and organization, separated by space (RIR). Empty for IP blocks not being announced in BGP
///     tables.
/// </param>
public record LocationModel(
    string Query,
    string Status,
    string Country,
    string CountryCode,
    string Region,
    string RegionName,
    string City,
    string Zip,
    float Lat,
    float Lon,
    string Timezone,
    string Isp,
    string Org,
    string As);