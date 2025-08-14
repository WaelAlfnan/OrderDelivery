using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Mappers;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace UnitTests.Mappers;

public class UserDtoMapperTests
{
    [Fact]
    public void ToUserDto_ValidUser_ReturnsCorrectDto()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "سارة",
            LastName = "علي",
            PhoneNumber = "0555555555",
            PhoneNumberConfirmed = true
        };
        var role = "Admin";
        // Act
        var dto = user.ToUserDto(role);
        // Assert
        Assert.Equal(user.Id.ToString(), dto.Id);
        Assert.Equal("سارة علي", dto.FullName);
        Assert.Equal(user.PhoneNumber, dto.PhoneNumber);
        Assert.True(dto.IsPhoneConfirmed);
        Assert.Equal(role, dto.Role);
    }

    [Fact]
    public void ToUserDto_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        ApplicationUser? user = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UserDtoMapper.ToUserDto(user!, "User"));
    }

    [Fact]
    public void ToUserDto_NullPhoneNumber_ReturnsEmptyString()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "سارة",
            LastName = "علي",
            PhoneNumber = null,
            PhoneNumberConfirmed = false
        };
        var role = "User";
        // Act
        var dto = user.ToUserDto(role);
        // Assert
        Assert.Equal(string.Empty, dto.PhoneNumber);
    }
} 