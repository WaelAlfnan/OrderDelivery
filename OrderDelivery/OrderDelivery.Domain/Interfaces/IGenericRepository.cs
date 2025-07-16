using System.Linq.Expressions;

namespace OrderDelivery.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations on entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        // Query Operations
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        
        // Query with Includes
        Task<T?> GetByIdWithIncludesAsync(object id, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        
        // Pagination
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, 
            int pageSize, 
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes);
        
        // CRUD Operations
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        
        // Raw SQL Operations (for complex queries)
        Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters);
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    }
}
