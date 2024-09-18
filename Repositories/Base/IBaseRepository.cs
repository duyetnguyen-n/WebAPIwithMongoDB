using WebAPIwithMongoDB.Entities;

namespace WebAPIwithMongoDB.Repositories.Base;

public interface IBaseRepository<TEntity> where TEntity : class, IMongoEntity
{
    Task<IEnumerable<TEntity>> GetAsync();
    Task<TEntity> GetAsync(string id);
    Task CreateAsync(TEntity obj);
    Task UpdateAsync(string id, TEntity obj);
    Task DeleteAsync(string id);
    Task DeleteAllAsync();
    Task<bool> Exists(string id);
}