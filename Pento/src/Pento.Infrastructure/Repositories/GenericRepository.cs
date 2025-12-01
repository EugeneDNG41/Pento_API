using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pento.Application.Abstractions.Data;


namespace Pento.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected ApplicationDbContext _context;
    protected DbSet<T> Table { get; set; }

    
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));  // Thêm kiểm tra null
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
    public virtual void Update(T entity)
    {
    EntityEntry<T> tracker = _context.Attach(entity);
        tracker.State = EntityState.Modified;
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        foreach (T entity in entities)
        {
            EntityEntry<T> tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }
    }
    public virtual void Remove(T entity)
    {
        _context.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _context.RemoveRange(entities);
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
