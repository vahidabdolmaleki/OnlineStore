using System.Collections;
using Shop.Domain.Common;
using Shop.Domain.Interfaces;
using Shop.Persistence.Context;
using Shop.Persistence.Repositories;

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

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
