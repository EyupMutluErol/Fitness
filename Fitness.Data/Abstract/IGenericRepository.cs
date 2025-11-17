using Fitness.Entities.Abstract;
using System.Linq.Expressions;

namespace Fitness.Data.Abstract;

public interface IGenericRepository<T> where T : class , IEntity , new()
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    Task<T> GetAsync(Expression<Func<T, bool>> filter);
}
