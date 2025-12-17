using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pento.Application.Abstractions.Persistence;
using ZiggyCreatures.Caching.Fusion;


namespace Pento.Infrastructure.Persistence;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected ApplicationDbContext _context;
    protected DbSet<T> Table { get; set; }

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        Table = _context.Set<T>();
    }
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Table.ToListAsync(cancellationToken);
    }
    public IQueryable<T> AsQueryable()
    {
        return Table.AsQueryable();
    }
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Table.FindAsync([id], cancellationToken);
    }
    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Table.FindAsync([id], cancellationToken);
    }
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Table.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await Table.CountAsync(cancellationToken);
        }

        return await Table.CountAsync(predicate, cancellationToken);
    }
    public async Task<int> CountDistinctAsync(Expression<Func<T, object>>? keySelector, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Table;
        if (predicate != null)
        {
            query = query.Where(predicate).OrderDescending();
        }
        if (keySelector != null)
        {
            return await query.Select(keySelector).Distinct().CountAsync(cancellationToken);
        }
        else
        {
            return await query.Distinct().CountAsync(cancellationToken);
        }

    }
    public virtual void Add(T entity)
    {
        _context.Add(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        _context.AddRange(entities);
    }
    public async virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> tracker = _context.Attach(entity);
        tracker.State = EntityState.Modified;
    }

    public async virtual Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (T entity in entities)
        {
            await UpdateAsync(entity, cancellationToken);
        }
    }
    public async virtual Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        MethodInfo? method = entity.GetType().GetMethod("Delete");
        if (method != null)
        {
            method.Invoke(entity, null);
            await UpdateAsync(entity, cancellationToken);
        } 
        else
        {
            _context.Remove(entity);
        }      
    }

    public async virtual Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (T entity in entities)
        {
            await RemoveAsync(entity, cancellationToken);
        }
    }

    public async Task<IEnumerable<T>> FindIncludeAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, CancellationToken cancellationToken = default)
    {
        return await Table.Where(predicate).Include(include).ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await Table.AnyAsync(cancellationToken);
        }

        return await Table.AnyAsync(predicate, cancellationToken);
    }
}
