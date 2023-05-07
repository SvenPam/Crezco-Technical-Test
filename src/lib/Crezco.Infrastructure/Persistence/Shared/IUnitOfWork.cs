namespace Crezco.Infrastructure.Persistence.Shared;

/// <summary>
///     Denotes a set of changes to be  completed as a single, cohesive task.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
}