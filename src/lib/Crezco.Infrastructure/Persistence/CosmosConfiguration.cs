namespace Crezco.Infrastructure.Persistence;

/// <summary>
///     Settings for using Cosmos DB.
/// </summary>
public sealed class CosmosConfiguration
{
    public string Endpoint { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
}