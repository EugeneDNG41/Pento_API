using System.Linq.Expressions;

namespace Pento.Application.Abstractions.Persistence;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(T entity, CancellationToken cancellationToken = default);
    IQueryable<T> AsQueryable();
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindIncludeAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> include, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    void AddRange(IEnumerable<T> entities);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<int> CountDistinctAsync(Expression<Func<T, object>>? keySelector, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
