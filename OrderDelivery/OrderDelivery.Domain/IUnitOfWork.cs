﻿using OrderDelivery.Domain.Interfaces;

namespace OrderDelivery.Domain
{
    /// <summary>
    /// Unit of Work interface for managing transactions and repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repository Access
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        
        // Transaction Management
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task<int> SaveChangesAsync();
        
        // Transaction Scope
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
        
        // Health Check
        Task<bool> CanConnectAsync();
    }
}
