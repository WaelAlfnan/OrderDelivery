# Repository Pattern Implementation

This document describes the implementation of the Repository Pattern and Unit of Work pattern in the OrderDelivery application, following best practices for .NET applications.

## Overview

The implementation provides a clean separation between the data access layer and business logic, making the code more maintainable, testable, and following SOLID principles.

## Architecture

### Interfaces

#### IGenericRepository<T>

A generic repository interface that provides common CRUD operations for any entity type:

```csharp
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
    
    // Raw SQL Operations
    Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters);
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
}
```

#### IUnitOfWork

Manages transactions and provides access to repositories:

```csharp
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
```

### Implementations

#### GenericRepository<T>

The concrete implementation of `IGenericRepository<T>` using Entity Framework Core:

- **Query Operations**: Basic CRUD operations with async support
- **Includes Support**: Eager loading of related entities
- **Pagination**: Built-in pagination with ordering and filtering
- **Raw SQL**: Support for complex queries when needed
- **Error Handling**: Proper null checks and argument validation

#### UnitOfWork

The concrete implementation of `IUnitOfWork`:

- **Repository Factory**: Creates and manages repository instances
- **Transaction Management**: Full transaction support with rollback capabilities
- **Transaction Scope**: Helper methods for executing operations within transactions
- **Resource Management**: Proper disposal of resources

## Usage Examples

### Basic Repository Usage

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Order> _orderRepository;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = unitOfWork.Repository<Order>();
    }

    // Get all orders
    public async Task<IEnumerable<Order>> GetAllOrdersAsync()
    {
        return await _orderRepository.GetAllAsync();
    }

    // Get order by ID with includes
    public async Task<Order?> GetOrderWithDetailsAsync(Guid orderId)
    {
        return await _orderRepository.GetByIdWithIncludesAsync(
            orderId, 
            o => o.Merchant, 
            o => o.Driver);
    }

    // Find orders with predicate
    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        return await _orderRepository.FindAsync(o => o.Status == OrderStatus.Pending);
    }
}
```

### Transaction Management

```csharp
public async Task<Order> CreateOrderWithTransactionAsync(Order order)
{
    return await _unitOfWork.ExecuteInTransactionAsync(async () =>
    {
        // Validate order
        if (order.OrderValue <= 0)
            throw new ArgumentException("Order value must be greater than zero.");

        // Set initial status
        order.Status = OrderStatus.Pending;
        order.PlacedAt = DateTime.UtcNow;

        // Add the order
        var createdOrder = await _orderRepository.AddAsync(order);
        
        return createdOrder;
    });
}
```

### Pagination and Filtering

```csharp
public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersPagedAsync(
    int pageNumber = 1, 
    int pageSize = 10, 
    OrderStatus? status = null)
{
    Expression<Func<Order, bool>>? predicate = null;
    
    if (status.HasValue)
    {
        predicate = order => order.Status == status.Value;
    }

    return await _orderRepository.GetPagedAsync(
        pageNumber: pageNumber,
        pageSize: pageSize,
        predicate: predicate,
        orderBy: o => o.PlacedAt,
        ascending: false,
        o => o.Merchant,
        o => o.Driver);
}
```

### Manual Transaction Control

```csharp
public async Task AssignDriverToOrderAsync(Guid orderId, Guid driverId)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new ArgumentException("Order not found.");

        order.DriverId = driverId;
        order.Status = OrderStatus.Assigned;
        order.AssignedAt = DateTime.UtcNow;

        _orderRepository.Update(order);

        await _unitOfWork.CommitAsync();
    }
    catch
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

## Dependency Injection Setup

The services are registered in `InfrastructureServiceExtensions.cs`:

```csharp
public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Register DbContext
    services.AddDbContext<OrderDeliveryDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("OrderDelivery.Infrastructure")));

    // Register repositories and unit of work
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
}
```

## Best Practices Implemented

### 1. **Separation of Concerns**
- Domain interfaces are separate from infrastructure implementations
- Business logic is isolated from data access concerns

### 2. **Async/Await Pattern**
- All database operations are async for better performance
- Proper use of `Task<T>` return types

### 3. **Error Handling**
- Comprehensive null checks and argument validation
- Proper exception handling in transaction scenarios
- Graceful error recovery with rollback capabilities

### 4. **Performance Optimization**
- Lazy loading of repositories (created on demand)
- Efficient query building with includes
- Pagination support for large datasets

### 5. **Resource Management**
- Proper implementation of `IDisposable`
- Transaction cleanup in finally blocks
- Memory-efficient repository caching

### 6. **Testability**
- Interface-based design enables easy mocking
- Dependency injection for loose coupling
- Isolated business logic for unit testing

### 7. **Type Safety**
- Generic constraints ensure type safety
- Expression trees for compile-time query validation
- Strong typing throughout the implementation

### 8. **Flexibility**
- Support for complex queries with includes
- Raw SQL support for performance-critical operations
- Configurable pagination and filtering

## Testing Considerations

### Unit Testing

```csharp
[Test]
public async Task CreateOrder_ValidOrder_ReturnsCreatedOrder()
{
    // Arrange
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockRepository = new Mock<IGenericRepository<Order>>();
    
    mockUnitOfWork.Setup(uow => uow.Repository<Order>())
        .Returns(mockRepository.Object);
    
    mockUnitOfWork.Setup(uow => uow.ExecuteInTransactionAsync(It.IsAny<Func<Task<Order>>>()))
        .Returns<Func<Task<Order>>>(async func => await func());
    
    var orderService = new OrderService(mockUnitOfWork.Object);
    var order = new Order { OrderValue = 100, CustomerName = "Test Customer" };

    // Act
    var result = await orderService.CreateOrderAsync(order);

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(OrderStatus.Pending, result.Status);
}
```

### Integration Testing

```csharp
[Test]
public async Task CreateOrder_WithRealDatabase_SavesToDatabase()
{
    // Arrange
    var options = new DbContextOptionsBuilder<OrderDeliveryDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    using var context = new OrderDeliveryDbContext(options);
    var unitOfWork = new UnitOfWork(context);
    var orderService = new OrderService(unitOfWork);

    var order = new Order 
    { 
        OrderValue = 100, 
        CustomerName = "Test Customer",
        MerchantId = Guid.NewGuid()
    };

    // Act
    var result = await orderService.CreateOrderAsync(order);

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(OrderStatus.Pending, result.Status);
    
    var savedOrder = await context.Orders.FindAsync(result.Id);
    Assert.IsNotNull(savedOrder);
}
```

## Performance Considerations

### 1. **Query Optimization**
- Use includes sparingly to avoid N+1 query problems
- Implement proper indexing on frequently queried columns
- Consider using projection for read-only operations

### 2. **Memory Management**
- Dispose of UnitOfWork properly in using statements
- Avoid keeping large result sets in memory
- Use pagination for large datasets

### 3. **Connection Management**
- Let Entity Framework handle connection pooling
- Use async operations to prevent thread blocking
- Implement proper timeout handling

## Security Considerations

### 1. **SQL Injection Prevention**
- Use parameterized queries (built into EF Core)
- Avoid raw SQL when possible
- Validate all user inputs

### 2. **Data Validation**
- Implement proper validation at the domain level
- Use Data Annotations or Fluent Validation
- Validate business rules in service layer

### 3. **Access Control**
- Implement proper authorization checks
- Use row-level security where appropriate
- Audit sensitive operations

## Migration and Maintenance

### 1. **Database Migrations**
- Use EF Core migrations for schema changes
- Test migrations in development environment
- Backup production data before migrations

### 2. **Version Control**
- Keep migrations in source control
- Document breaking changes
- Use semantic versioning for releases

### 3. **Monitoring**
- Implement logging for database operations
- Monitor query performance
- Set up alerts for database issues

## Conclusion

This implementation provides a robust, scalable, and maintainable data access layer that follows industry best practices. The combination of the Repository Pattern and Unit of Work pattern creates a clean separation of concerns while providing powerful transaction management capabilities.

The implementation is designed to be:
- **Maintainable**: Clear separation of concerns and well-documented code
- **Testable**: Interface-based design enables comprehensive testing
- **Performant**: Async operations and efficient query patterns
- **Secure**: Proper validation and error handling
- **Flexible**: Support for complex scenarios while maintaining simplicity

This foundation will support the growth of the OrderDelivery application while maintaining code quality and developer productivity. 