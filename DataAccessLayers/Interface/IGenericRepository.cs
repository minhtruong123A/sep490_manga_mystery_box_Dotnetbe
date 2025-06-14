using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindOneAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(string id);
        Task DeleteRangeAsync(Expression<Func<T, bool>> expression);
        Task DeleteAllAsync();
        Task UpdateAsync(string id, T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
    }
}
