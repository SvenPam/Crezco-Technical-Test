using System.Net;

namespace IPApi.Client.Locations;

/// <summary>
///     Describes an issue when failing to make a request to IP API.
/// </summary>
public sealed class IpApiRequestException : HttpRequestException
{
    internal IpApiRequestException(string requestedIp, HttpStatusCode responseStatusCode, string response) : base(
        $"Failed to make request for IP {requestedIp}.", null, responseStatusCode)
    {
        this.RequestedIp = requestedIp;
        this.Response = response;
    }

    /// <summary>
    ///     The initially requested IP address.
    /// </summary>
    public string RequestedIp { get; }

    /// <summary>
    ///     The IP API response content.
    /// </summary>
    public string Response { get; }
}