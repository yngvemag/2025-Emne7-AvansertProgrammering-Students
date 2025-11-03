namespace StudentBloggAPI.Features.Common.Interfaces;

public interface IBaseService<T> 
    where T : class
{
    Task<T?> AddAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> DeleteByIdAsync(Guid id);
    Task<T?> UpdateAsync(Guid id, T entity);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
}