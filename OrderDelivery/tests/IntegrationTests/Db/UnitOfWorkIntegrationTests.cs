using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Infrastructure.Data.Contexts;
using OrderDelivery.Infrastructure.Data;
using Xunit;

namespace IntegrationTests.Db;

public class UnitOfWorkIntegrationTests
{
    [Fact]
    public async Task AddAndGet_Entity_WithUnitOfWork_Success()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<OrderDeliveryDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrderDeliveryDb_{Guid.NewGuid()}")
            .Options;
        using var context = new OrderDeliveryDbContext(options);
        var unitOfWork = new UnitOfWork(context);
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "Integration",
            LastName = "Test",
            PhoneNumber = "0555555555"
        };
        // Act
        await unitOfWork.Repository<ApplicationUser>().AddAsync(user);
        await unitOfWork.SaveChangesAsync();
        var fetched = await unitOfWork.Repository<ApplicationUser>().GetByIdAsync(user.Id);
        // Assert
        Assert.NotNull(fetched);
        Assert.Equal(user.FirstName, fetched!.FirstName);
        Assert.Equal(user.LastName, fetched.LastName);
        Assert.Equal(user.PhoneNumber, fetched.PhoneNumber);
    }
} 