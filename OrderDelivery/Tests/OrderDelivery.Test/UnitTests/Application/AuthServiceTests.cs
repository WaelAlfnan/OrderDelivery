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

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<ISmsService> _mockSmsService;
    private readonly Mock<IRefreshTokenService> _mockRefreshTokenService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
            _mockUserManager.Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null!, null!, null!, null!);
        _mockJwtService = new Mock<IJwtService>();
        _mockSmsService = new Mock<ISmsService>();
        _mockRefreshTokenService = new Mock<IRefreshTokenService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockJwtService.Object,
            _mockSmsService.Object,
            _mockRefreshTokenService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessAndAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var roles = new List<string> { "Driver" };
        var userDto = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var accessToken = "access-token";
        var refreshToken = "refresh-token";

        _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.PhoneNumber))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<UserDto>(), roles))
            .Returns(accessToken);
        _mockRefreshTokenService.Setup(x => x.CreateRefreshTokenAsync(user.Id, null))
            .ReturnsAsync(refreshToken);

        // Act
        var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.True(success);
        Assert.Equal("Login successful.", message);
        Assert.NotNull(authResponse);
        Assert.Equal(accessToken, authResponse!.AccessToken);
        Assert.Equal(refreshToken, authResponse.RefreshToken);
        Assert.NotNull(authResponse.User);
        Assert.Equal("أحمد علي", authResponse.User.FullName);
    }

    [Fact]
    public async Task LoginAsync_InvalidPhoneNumber_ReturnsFailure()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");

        _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.PhoneNumber))
            .ReturnsAsync((ApplicationUser)null!);

        // Act
        var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(success);
        Assert.Equal("Invalid phone number or password.", message);
        Assert.Null(authResponse);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "wrongpassword");
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.PhoneNumber))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // Act
        var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(success);
        Assert.Equal("Invalid phone number or password.", message);
        Assert.Null(authResponse);
    }

    [Fact]
    public async Task LoginAsync_ExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");

        _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.PhoneNumber))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.False(success);
        Assert.Equal("An error occurred during login.", message);
        Assert.Null(authResponse);
    }

    [Fact]
    public async Task LoginAsync_UserWithNoRoles_ReturnsSuccessWithEmptyRole()
    {
        // Arrange
        var loginDto = new LoginDto("+966501234567", "password123");
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = "أحمد",
            LastName = "علي",
            PhoneNumber = "+966501234567",
            PhoneNumberConfirmed = true
        };
        var roles = new List<string>();
        var accessToken = "access-token";
        var refreshToken = "refresh-token";

        _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.PhoneNumber))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<UserDto>(), roles))
            .Returns(accessToken);
        _mockRefreshTokenService.Setup(x => x.CreateRefreshTokenAsync(user.Id, null))
            .ReturnsAsync(refreshToken);

        // Act
        var (success, message, authResponse) = await _authService.LoginAsync(loginDto);

        // Assert
        Assert.True(success);
        Assert.Equal("Login successful.", message);
        Assert.NotNull(authResponse);
        Assert.Equal("", authResponse!.User.Role);
    }

    [Fact]
    public async Task LogoutAsync_ValidUserId_ReturnsSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockRefreshTokenService.Setup(x => x.RevokeAllRefreshTokensAsync(userId))
            .ReturnsAsync(true);

        // Act
        var (success, message) = await _authService.LogoutAsync(userId);

        // Assert
        Assert.True(success);
        Assert.Equal("Logout successful.", message);
        _mockRefreshTokenService.Verify(x => x.RevokeAllRefreshTokensAsync(userId), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockRefreshTokenService.Setup(x => x.RevokeAllRefreshTokensAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (success, message) = await _authService.LogoutAsync(userId);

        // Assert
        Assert.False(success);
        Assert.Equal("An error occurred during logout.", message);
    }
}
