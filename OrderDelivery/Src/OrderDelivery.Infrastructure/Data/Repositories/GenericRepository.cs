using Microsoft.EntityFrameworkCore;
using OrderDelivery.Domain.Interfaces;
using System.Linq.Expressions;

namespace OrderDelivery.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Generic repository implementation for Entity Framework Core
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        #region Query Operations

        /// <summary>
        /// Gets an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity or null if not found.</returns>
        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets an entity by its ID without tracking.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity or null if not found.</returns>
        public virtual async Task<T?> GetByIdAsNoTrackingAsync(object id)
        {
            // This assumes 'Id' is the primary key property name. Adjust if different.
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
        }

        /// <summary>
        /// Gets all entities of a specific type without tracking.
        /// </summary>
        /// <returns>An enumerable of all entities.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Finds entities based on a predicate without tracking.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An enumerable of entities matching the predicate.</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Gets the first entity that satisfies a predicate without tracking.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The first entity matching the predicate, or null if no such entity is found.</returns>
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Determines whether any entity satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>True if any entities satisfy the condition; otherwise, false.</returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Returns the number of entities that satisfy a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The number of entities that satisfy the condition.</returns>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Returns the total number of entities.
        /// </summary>
        /// <returns>The total number of entities.</returns>
        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        #endregion

        #region Query with Includes

        /// <summary>
        /// Gets an entity by its ID with specified related entities loaded.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <param name="includes">Expressions for the related entities to include.</param>
        /// <returns>The entity with included navigation properties, or null if not found.</returns>
        public virtual async Task<T?> GetByIdWithIncludesAsync(object id, params Expression<Func<T, object>>[] includes)
        {
            // Note: FindAsync does not support AsNoTracking directly for ID lookup.
            // This implementation assumes a tracked entity or needs modification for AsNoTracking by ID.
            var query = _dbSet.AsQueryable(); // Default is tracking for this method

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            // You might need a more robust way to find by ID after includes,
            // e.g., if the ID property name is not "Id"
            return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
        }

        /// <summary>
        /// Gets all entities with specified related entities loaded without tracking.
        /// </summary>
        /// <param name="includes">Expressions for the related entities to include.</param>
        /// <returns>An enumerable of entities with included navigation properties.</returns>
        public virtual async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Finds entities based on a predicate with specified related entities loaded without tracking.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="includes">Expressions for the related entities to include.</param>
        /// <returns>An enumerable of entities matching the predicate with included navigation properties.</returns>
        public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking().Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        #endregion

        #region Pagination

        /// <summary>
        /// Gets a paged list of entities with optional filtering, ordering, and includes.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="predicate">Optional: A function to test each element for a condition.</param>
        /// <param name="orderBy">Optional: An expression to order the entities by.</param>
        /// <param name="ascending">True for ascending order, false for descending.</param>
        /// <param name="includes">Optional: Expressions for the related entities to include.</param>
        /// <returns>A tuple containing the paged items and the total count of matching entities.</returns>
        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, object>>? orderBy = null,
            bool ascending = true,
            params Expression<Func<T, object>>[] includes)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        /// <summary>
        /// Adds a range of new entities to the database.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <returns>The added entities.</returns>
        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var entityList = entities.ToList();
            await _dbSet.AddRangeAsync(entityList);
            return entityList;
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        /// <summary>
        /// Updates a range of existing entities in the database.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
        }

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public virtual void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Removes a range of entities from the database.
        /// </summary>
        /// <param name="entities">The entities to remove.</param>
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }

        #endregion

        #region Raw SQL Operations

        /// <summary>
        /// Executes a raw SQL query and returns entities.
        /// Results are tracked by default, use .AsNoTracking() for read-only.
        /// </summary>
        /// <param name="sql">The raw SQL query string.</param>
        /// <param name="parameters">Parameters to apply to the SQL query.</param>
        /// <returns>An enumerable of entities mapped from the query results.</returns>
        public virtual async Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters)
        {
            return await _dbSet.FromSqlRaw(sql, parameters).ToListAsync();
        }

        /// <summary>
        /// Executes a raw SQL command (INSERT, UPDATE, DELETE, etc.) on the database.
        /// </summary>
        /// <param name="sql">The raw SQL command string.</param>
        /// <param name="parameters">Parameters to apply to the SQL command.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public virtual async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        #endregion
    }
}