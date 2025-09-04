using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Services;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Application;

public class RefreshTokenServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ILogger<RefreshTokenService>> _mockLogger;
    private readonly RefreshTokenService _refreshTokenService;

    public RefreshTokenServiceTests()
    {
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _mockJwtService = new Mock<IJwtService>();
        _mockLogger = new Mock<ILogger<RefreshTokenService>>();

        _refreshTokenService = new RefreshTokenService(
            _mockUserManager.Object,
            _mockJwtService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ValidUserId_ReturnsRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };
        var refreshTokenString = "refresh-token-string";

        _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshTokenString);

        // Act
        var result = await _refreshTokenService.CreateRefreshTokenAsync(userId);

        // Assert
        Assert.Equal(refreshTokenString, result);
        _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
        _mockJwtService.Verify(x => x.GenerateRefreshToken(), Times.Once);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ValidUserIdWithIpAddress_ReturnsRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ipAddress = "192.168.1.1";
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567"
        };
        var refreshTokenString = "refresh-token-string";

        _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns(refreshTokenString);

        // Act
        var result = await _refreshTokenService.CreateRefreshTokenAsync(userId, ipAddress);

        // Assert
        Assert.Equal(refreshTokenString, result);
        _mockUserManager.Verify(x => x.FindByIdAsync(userId.ToString()), Times.Once);
        _mockJwtService.Verify(x => x.GenerateRefreshToken(), Times.Once);
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync((ApplicationUser)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _refreshTokenService.CreateRefreshTokenAsync(userId));
        
        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task RefreshTokenAsync_ValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange
        var refreshTokenRequest = new RefreshTokenRequest("valid-refresh-token");
        var userId = Guid.NewGuid();
        var user = new ApplicationUser
        {
            Id = userId,
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var roles = new List<string> { "Driver" };
        var userDto = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var newAccessToken = "new-access-token";
        var newRefreshToken = "new-refresh-token";

        _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<UserDto>(), roles))
            .Returns(newAccessToken);
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns(newRefreshToken);

        // Act
        var result = await _refreshTokenService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.AuthResponse);
        Assert.Equal(newAccessToken, result.AuthResponse.AccessToken);
        Assert.Equal(newRefreshToken, result.AuthResponse.RefreshToken);
        Assert.NotNull(result.AuthResponse.User);
        Assert.Equal("أحمد علي", result.AuthResponse.User.FullName);
    }

    [Fact]
    public async Task RefreshTokenAsync_InvalidRefreshToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var refreshTokenRequest = new RefreshTokenRequest("invalid-refresh-token");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _refreshTokenService.RefreshTokenAsync(refreshTokenRequest.RefreshToken));
        
        Assert.Equal("Invalid refresh token", exception.Message);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";

        // Act
        var result = await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";

        // Act
        var result = await _refreshTokenService.RevokeRefreshTokenAsync(refreshToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RevokeAllRefreshTokensAsync_ValidUserId_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _refreshTokenService.RevokeAllRefreshTokensAsync(userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RevokeAllRefreshTokensAsync_InvalidUserId_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _refreshTokenService.RevokeAllRefreshTokensAsync(userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRefreshTokenValidAsync_ValidToken_ReturnsTrue()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";

        // Act
        var result = await _refreshTokenService.IsRefreshTokenValidAsync(refreshToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsRefreshTokenValidAsync_InvalidToken_ReturnsFalse()
    {
        // Arrange
        var refreshToken = "invalid-refresh-token";

        // Act
        var result = await _refreshTokenService.IsRefreshTokenValidAsync(refreshToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsRefreshTokenValidAsync_ExpiredToken_ReturnsFalse()
    {
        // Arrange
        var refreshToken = "expired-refresh-token";

        // Act
        var result = await _refreshTokenService.IsRefreshTokenValidAsync(refreshToken);

        // Assert
        Assert.False(result);
    }
}