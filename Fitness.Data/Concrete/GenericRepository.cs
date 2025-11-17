using Fitness.Data.Abstract;
using Fitness.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fitness.Data.Concrete;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntityWithId, new()
{
    protected readonly DbContext _context;

    public GenericRepository(DbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }
    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _context.Set<T>().AsNoTracking().Where(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(filter);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    
}
