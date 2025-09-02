namespace PersonRestAPI.Repository.Interfaces;

public interface IRepository<T>
{
    Task<T?> AddAsync(T item);
    Task<T?> GetByIdAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> UpdateAsync(T item);
    Task<bool> DeleteAsync(long id);
}