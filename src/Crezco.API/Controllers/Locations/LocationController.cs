using System.Net;
using Crezco.Application.Locations.Query;
using Crezco.Application.Shared;
using Crezco.Shared.Locations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Crezco.API.Controllers.Locations;

/// <summary>
///     Responsible for obtaining locations.
/// </summary>
[Route("[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initialize a new instance of <see cref="LocationController"/>.
    /// </summary>
    /// <param name="mediator"></param>
    public LocationController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    ///     Returns a location for a given IP.
    /// </summary>
    /// <param name="ipAddress">A valid IP address.</param>
    /// <returns>A location for the provided IP.</returns>
    /// <returns>Existing appointment data in an Appointment object or a business error.</returns>
    /// <response code="200">Location for IP found.</response>
    /// <response code="400">IP Address is invalid.</response>
    /// <remarks>
    ///    Should time have allowed, this endpoint should ideally be authenticated and rate limited.
    ///    My approach to this would, in a service to service or API key scenario be authentication via
    ///    an APIM.
    /// </remarks>
    [HttpGet("ip/{ipAddress}")]
    [ProducesResponseType(typeof(Response<Location>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Response<Location>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetLocationForIp([FromRoute] string ipAddress) =>
        this.Ok(
            await this._mediator.Send(new GetLocationFromIp.Query(ipAddress.Trim()))
        );
}