using Microsoft.EntityFrameworkCore;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Infrastructure.Data.Contexts;
using OrderDelivery.Infrastructure.Data;
using Xunit;

namespace OrderDelivery.UnitTests.Infrastructure;

public class UnitOfWorkTests : IDisposable
{
    private readonly OrderDeliveryDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<OrderDeliveryDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrderDeliveryDb_{Guid.NewGuid()}")
            .Options;

        _context = new OrderDeliveryDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void Repository_GetRepository_ReturnsGenericRepository()
    {
        // Act
        var repository = _unitOfWork.Repository<ApplicationUser>();

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public void Repository_GetSameRepositoryTwice_ReturnsSameInstance()
    {
        // Act
        var repository1 = _unitOfWork.Repository<ApplicationUser>();
        var repository2 = _unitOfWork.Repository<ApplicationUser>();

        // Assert
        Assert.Same(repository1, repository2);
    }

    [Fact]
    public void Repository_GetDifferentRepositories_ReturnsDifferentInstances()
    {
        // Act
        var userRepository = _unitOfWork.Repository<ApplicationUser>();
        var driverRepository = _unitOfWork.Repository<Driver>();

        // Assert
        Assert.NotSame(userRepository, driverRepository);
    }

    [Fact]
    public async Task SaveChangesAsync_WithChanges_SavesChangesToDatabase()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        var repository = _unitOfWork.Repository<ApplicationUser>();
        await repository.AddAsync(user);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("أحمد", savedUser!.FirstName);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutChanges_ReturnsZero()
    {
        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleRepositories_SavesAllChanges()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = user.Id,
            VehicleType = Domain.Enums.VehicleType.Motorcycle,
            IsAvailable = true,
            Rating = 0,
            TotalDeliveries = 0,
            CurrentLatitude = 24.7136m,
            CurrentLongitude = 46.6753m
        };

        var userRepository = _unitOfWork.Repository<ApplicationUser>();
        var driverRepository = _unitOfWork.Repository<Driver>();

        await userRepository.AddAsync(user);
        await driverRepository.AddAsync(driver);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        var savedUser = await _context.Users.FindAsync(user.Id);
        var savedDriver = await _context.Drivers.FindAsync(driver.Id);
        
        Assert.NotNull(savedUser);
        Assert.NotNull(savedDriver);
        Assert.Equal("أحمد", savedUser!.FirstName);
        Assert.Equal(Domain.Enums.VehicleType.Motorcycle, savedDriver!.VehicleType);
    }

    [Fact]
    public async Task SaveChangesAsync_WithTransaction_CommitsAllChanges()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        var driver = new Driver
        {
            Id = Guid.NewGuid(),
            ApplicationUserId = user.Id,
            VehicleType = Domain.Enums.VehicleType.Motorcycle,
            IsAvailable = true,
            Rating = 0,
            TotalDeliveries = 0,
            CurrentLatitude = 24.7136m,
            CurrentLongitude = 46.6753m
        };

        var userRepository = _unitOfWork.Repository<ApplicationUser>();
        var driverRepository = _unitOfWork.Repository<Driver>();

        await userRepository.AddAsync(user);
        await driverRepository.AddAsync(driver);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        
        // Verify both entities were saved
        var savedUser = await _context.Users.FindAsync(user.Id);
        var savedDriver = await _context.Drivers.FindAsync(driver.Id);
        
        Assert.NotNull(savedUser);
        Assert.NotNull(savedDriver);
    }

    [Fact]
    public async Task SaveChangesAsync_WithInvalidData_ThrowsException()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "invalid-phone" // This should cause validation error
        };

        var repository = _unitOfWork.Repository<ApplicationUser>();
        await repository.AddAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => _unitOfWork.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_WithConcurrentModifications_HandlesConcurrency()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        var repository = _unitOfWork.Repository<ApplicationUser>();
        await repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Simulate concurrent modification
        var userFromDb = await repository.GetByIdAsync(user.Id);
        userFromDb!.FirstName = "محمد";

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("محمد", updatedUser!.FirstName);
    }

    [Fact]
    public async Task SaveChangesAsync_WithLargeDataSet_HandlesPerformance()
    {
        // Arrange
        var users = new List<ApplicationUser>();
        for (int i = 0; i < 100; i++)
        {
            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = $"User{i}",
                LastName = "Test",
                PhoneNumber = $"+966501234{i:D3}"
            });
        }

        var repository = _unitOfWork.Repository<ApplicationUser>();
        await repository.AddRangeAsync(users);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.True(result > 0);
        var savedUsers = await _context.Users.ToListAsync();
        Assert.Equal(100, savedUsers.Count);
    }

    [Fact]
    public void Dispose_DisposesContext()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        // The context should be disposed and throw exception on access
        Assert.Throws<ObjectDisposedException>(() => _context.Users.Count());
    }

    [Fact]
    public async Task SaveChangesAsync_AfterDispose_ThrowsException()
    {
        // Arrange
        _unitOfWork.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _unitOfWork.SaveChangesAsync());
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}
