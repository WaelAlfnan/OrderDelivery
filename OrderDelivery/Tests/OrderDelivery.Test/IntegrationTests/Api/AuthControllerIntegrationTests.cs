using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Api;
using Xunit;

namespace IntegrationTests.Api;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<IAuthService> _mockAuthService;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the auth service
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                _mockAuthService = new Mock<IAuthService>();
                services.AddScoped<IAuthService>(_ => _mockAuthService.Object);
            });
        });
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto("+966501234567", "password123");
        var userDto = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var authResponse = new AuthResponseDto("access-token", "refresh-token", DateTime.UtcNow.AddMinutes(60), userDto);

        _mockAuthService.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync((true, "Login successful.", authResponse));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("access-token", apiResponse.Data!.AccessToken);
        Assert.Equal("refresh-token", apiResponse.Data.RefreshToken);
        Assert.Equal("أحمد علي", apiResponse.Data.User.FullName);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto("+966501234567", "wrongpassword");

        _mockAuthService.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync((false, "Invalid phone number or password.", null));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Invalid phone number or password.", apiResponse.Message);
    }

    [Fact]
    public async Task Login_EmptyPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto("", "password123");

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_EmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto("+966501234567", "");

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new LoginDto("+966501234567", "password123");

        _mockAuthService.Setup(x => x.LoginAsync(loginDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("An internal server error occurred", apiResponse.Message);
    }

    [Fact]
    public async Task Logout_ValidUserId_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();

        _mockAuthService.Setup(x => x.LogoutAsync(userId))
            .ReturnsAsync((true, "Logout successful."));

        // Act
        var response = await client.PostAsJsonAsync($"/api/Auth/logout/{userId}", new { });

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Logout successful.", apiResponse.Message);
    }

    [Fact]
    public async Task Logout_InvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();

        _mockAuthService.Setup(x => x.LogoutAsync(userId))
            .ReturnsAsync((false, "User not found."));

        // Act
        var response = await client.PostAsJsonAsync($"/api/Auth/logout/{userId}", new { });

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("User not found.", apiResponse.Message);
    }

    [Fact]
    public async Task RefreshToken_ValidRefreshToken_ReturnsNewTokens()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("valid-refresh-token");
        var userDto = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var authResponse = new AuthResponseDto("new-access-token", "new-refresh-token", DateTime.UtcNow.AddMinutes(60), userDto);

        _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync((true, "Token refreshed successfully.", authResponse));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/refresh-token", refreshTokenRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("new-access-token", apiResponse.Data!.AccessToken);
        Assert.Equal("new-refresh-token", apiResponse.Data.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("invalid-refresh-token");

        _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync((false, "Invalid refresh token.", null));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/refresh-token", refreshTokenRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Invalid refresh token.", apiResponse.Message);
    }

    [Fact]
    public async Task RefreshToken_EmptyRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("");

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/refresh-token", refreshTokenRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
