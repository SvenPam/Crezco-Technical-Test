using System.Net;
using Crezco.Application.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Crezco.API.Filters;

/// <summary>
///     Filter for standardizing HTTP response codes to application ones.
/// </summary>
public class ResponseFilter : IActionFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult { Value: Response response } ||
            response.Status == Status.Success) return;
        var code = response.Status switch
        {
            Status.Invalid => HttpStatusCode.BadRequest,
            Status.Success => HttpStatusCode.OK,
            _ => HttpStatusCode.InternalServerError
        };

        context.Result = new ObjectResult(new
        {
            response.Errors
        })
        {
            StatusCode = (int)code
        };
    }
}