using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Project_Gon.Infrastructure.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

  
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<T?> GetByIdAsync(int id);

    Task<T?> GetByIdAsync(int id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteAsync(T entity);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}