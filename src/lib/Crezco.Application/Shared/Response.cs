namespace Crezco.Application.Shared;

/// <summary>
///     Represents a generic response wrapper.
/// </summary>
/// <remarks>
///     Providing a defined response allows standardization across the application.
///     Using a custom defined status of how the request was handled allows us not
///     to only cater for HTTP/API but other application types. For example,
///     we may wish to handle events in a function through the same application
///     and domain logic.
/// </remarks>
public class Response<TResult> : Response
    where TResult : new()
{
    public TResult? Result { get; set; }
}

/// <summary>
///     Represents a generic response wrapper without a result.
/// </summary>
public class Response
{
    /// <summary>
    ///     The status of this response.
    /// </summary>
    public Status Status { get; init; }

    /// <summary>
    ///     Errors related to the request which requested this response.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
}

/// <summary>
///     Application response status.
/// </summary>
public enum Status
{
    Success,
    Invalid
}