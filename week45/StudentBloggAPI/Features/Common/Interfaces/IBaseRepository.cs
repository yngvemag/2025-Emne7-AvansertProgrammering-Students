using System.Linq.Expressions;

namespace StudentBloggAPI.Features.Common.Interfaces;

public interface IBaseRepository<T> 
    where T : class
{
    Task<T?> AddAsync(T entity);
    Task<T?> UpdateAsync(Guid id, T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> DeleteByIdAsync(Guid id);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
    
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}