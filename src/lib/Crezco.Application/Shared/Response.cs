namespace Crezco.Application.Shared;

/// <summary>
///     Represents a generic response wrapper.
/// </summary>
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