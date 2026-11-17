using Microsoft.EntityFrameworkCore;
using Shop.Domain.Common;
using Shop.Domain.Interfaces;
using Shop.Persistence.Context;
using Shop.Persistence.Repositories;
using System.Collections;
using System.Data;

namespace Shop.Persistence.UnitOfWork;

public class UnitOfWork(ShopDbContext context) : IUnitOfWork
{
    private readonly ShopDbContext _context = context;
    private Hashtable? _repositories;

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        _repositories ??= [];


        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IGenericRepository<T>)_repositories[type]!;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task ExecuteInTransactionAsync(
       Func<CancellationToken, Task> action,
       IsolationLevel isolationLevel = IsolationLevel.Serializable,
       CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        // در صورت فعال بودن retry در EF، کل تراکنش دوباره اجرا می‌شود.
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    isolationLevel,
                    cancellationToken);

            try
            {
                await action(cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                // تغییرات ردیابی‌شده پس از شکست تراکنش نباید باقی بمانند.
                // DisposeAsync تراکنشِ commitنشده را rollback می‌کند.
                _context.ChangeTracker.Clear();
                throw;
            }
        });
    }
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
