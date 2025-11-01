
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Abstractions.Data;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
    IQueryable<T> AsQueryable();
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindIncludeAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<T> entities);
    void UpdateRange(IEnumerable<T> entities);
    void RemoveRange(IEnumerable<T> entities);
}
