using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderDelivery.Domain;
using OrderDelivery.Domain.Interfaces;
using OrderDelivery.Infrastructure.Data.Contexts;
using OrderDelivery.Infrastructure.Data.Repositories;

namespace OrderDelivery.Infrastructure.Data
{
    /// <summary>
    /// Unit of Work implementation for managing transactions and repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderDeliveryDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed;

        public UnitOfWork(OrderDeliveryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();
        }

        #region Repository Access

        /// <summary>
        /// Gets or creates a repository for the specified entity type
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The repository instance</returns>
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>).MakeGenericType(type);
                var repository = Activator.CreateInstance(repositoryType, _context);
                _repositories[type] = repository!;
            }

            return (IGenericRepository<TEntity>)_repositories[type];
        }

        #endregion

        #region Transaction Management

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public async Task CommitAsync()
        {
            try
            {
                if (_currentTransaction == null)
                {
                    throw new InvalidOperationException("No transaction to commit.");
                }

                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Rolls back the current transaction
        /// </summary>
        public async Task RollbackAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Saves all changes to the database
        /// </summary>
        /// <returns>The number of affected entries</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion

        #region Transaction Scope

        /// <summary>
        /// Executes an operation within a transaction scope
        /// </summary>
        /// <typeparam name="TResult">The result type</typeparam>
        /// <param name="operation">The operation to execute</param>
        /// <returns>The operation result</returns>
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Executes an operation within a transaction scope
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await operation();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Health Check

        /// <summary>
        /// Checks if the database connection is available
        /// </summary>
        /// <returns>True if connection is available, false otherwise</returns>
        public async Task<bool> CanConnectAsync()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes the unit of work and its resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the unit of work and its resources
        /// </summary>
        /// <param name="disposing">True if disposing, false if finalizing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                _context?.Dispose();
                _repositories.Clear();
            }

            _disposed = true;
        }

        #endregion
    }
}
