using Shop.Domain.Common;
using System.Data;

namespace Shop.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> CompleteAsync();

    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default);
}
