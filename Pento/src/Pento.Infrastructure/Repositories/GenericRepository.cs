
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pento.Application.Abstractions.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
}
