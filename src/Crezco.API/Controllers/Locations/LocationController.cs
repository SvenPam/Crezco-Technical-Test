using Crezco.Application.Locations.Query;
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
    [HttpGet("ip/{ipAddress}")]
    public async Task<IActionResult> GetLocationForIp([FromRoute] string ipAddress) =>
        this.Ok(
            await this._mediator.Send(new GetLocationFromIp.Query(ipAddress))
        );
}