namespace Kmila.Shared.Interfaces;

public interface IRepository<T> : IDisposable
{
    Task<List<T>> GetListAsync();
    Task<T> GetByAsync(object id);
    Task<bool> UpdateAsync(T item);
    Task<bool> RemoveAsync(T item);
    Task<bool> CreateAsync(T item);
}