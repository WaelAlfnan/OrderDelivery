using Microsoft.EntityFrameworkCore;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Infrastructure.Data.Contexts;
using OrderDelivery.Infrastructure.Data.Repositories;
using Xunit;

namespace OrderDelivery.UnitTests.Infrastructure;

public class GenericRepositoryTests : IDisposable
{
    private readonly OrderDeliveryDbContext _context;
    private readonly GenericRepository<ApplicationUser> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrderDeliveryDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrderDeliveryDb_{Guid.NewGuid()}")
            .Options;

        _context = new OrderDeliveryDbContext(options);
        _repository = new GenericRepository<ApplicationUser>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingEntity_ReturnsEntity()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("أحمد", result.FirstName);
        Assert.Equal("علي", result.LastName);
        Assert.Equal("+966501234567", result.PhoneNumber);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntity_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsNoTrackingAsync_ExistingEntity_ReturnsEntityWithoutTracking()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsNoTrackingAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("أحمد", result.FirstName);
        Assert.Equal("علي", result.LastName);
        Assert.Equal("+966501234567", result.PhoneNumber);
    }

    [Fact]
    public async Task GetAllAsync_WithEntities_ReturnsAllEntities()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.FirstName == "أحمد");
        Assert.Contains(result, u => u.FirstName == "محمد");
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindAsync_WithMatchingCondition_ReturnsMatchingEntities()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(u => u.FirstName == "أحمد");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("أحمد", result.First().FirstName);
    }

    [Fact]
    public async Task FindAsync_WithNoMatchingCondition_ReturnsEmptyCollection()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(u => u.FirstName == "محمد");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMatchingCondition_ReturnsFirstMatchingEntity()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "محمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(u => u.FirstName == "أحمد");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("أحمد", result!.FirstName);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNoMatchingCondition_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(u => u.FirstName == "محمد");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidEntity_AddsEntityToDatabase()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        // Act
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(savedUser);
        Assert.Equal("أحمد", savedUser!.FirstName);
        Assert.Equal("علي", savedUser.LastName);
        Assert.Equal("+966501234567", savedUser.PhoneNumber);
    }

    [Fact]
    public async Task AddRangeAsync_ValidEntities_AddsAllEntitiesToDatabase()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        // Act
        await _repository.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Assert
        var savedUsers = await _context.Users.ToListAsync();
        Assert.Equal(2, savedUsers.Count);
        Assert.Contains(savedUsers, u => u.FirstName == "أحمد");
        Assert.Contains(savedUsers, u => u.FirstName == "محمد");
    }

    [Fact]
    public async Task Update_ExistingEntity_UpdatesEntityInDatabase()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        user.FirstName = "محمد";
        user.LastName = "أحمد";
        _repository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("محمد", updatedUser!.FirstName);
        Assert.Equal("أحمد", updatedUser.LastName);
    }

    [Fact]
    public async Task UpdateRange_ExistingEntities_UpdatesAllEntitiesInDatabase()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        users[0].FirstName = "سعد";
        users[1].FirstName = "خالد";
        _repository.UpdateRange(users);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUsers = await _context.Users.ToListAsync();
        Assert.Equal(2, updatedUsers.Count);
        Assert.Contains(updatedUsers, u => u.FirstName == "سعد");
        Assert.Contains(updatedUsers, u => u.FirstName == "خالد");
    }

    [Fact]
    public async Task Remove_ExistingEntity_RemovesEntityFromDatabase()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(user);
        await _context.SaveChangesAsync();

        // Assert
        var removedUser = await _context.Users.FindAsync(user.Id);
        Assert.Null(removedUser);
    }

    [Fact]
    public async Task RemoveRange_ExistingEntities_RemovesAllEntitiesFromDatabase()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        _repository.RemoveRange(users);
        await _context.SaveChangesAsync();

        // Assert
        var remainingUsers = await _context.Users.ToListAsync();
        Assert.Empty(remainingUsers);
    }

    [Fact]
    public async Task CountAsync_WithEntities_ReturnsCorrectCount()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountAsync_EmptyDatabase_ReturnsZero()
    {
        // Act
        var count = await _repository.CountAsync();

        // Assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CountAsync_WithCondition_ReturnsCorrectCount()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "أحمد",
                LastName = "علي",
                PhoneNumber = "+966501234567"
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FirstName = "محمد",
                LastName = "أحمد",
                PhoneNumber = "+966501234568"
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var count = await _repository.CountAsync(u => u.FirstName == "أحمد");

        // Assert
        Assert.Equal(1, count);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
