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
    [HttpGet("ip/{ipAddress}")]
    [ProducesResponseType(typeof(Response<Location>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(Response<Location>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetLocationForIp([FromRoute] string ipAddress) =>
        this.Ok(
            await this._mediator.Send(new GetLocationFromIp.Query(ipAddress.Trim()))
        );
}