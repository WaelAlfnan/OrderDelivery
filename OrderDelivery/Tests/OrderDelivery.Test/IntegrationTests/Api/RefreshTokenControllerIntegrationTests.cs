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

public class RefreshTokenControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<IRefreshTokenService> _mockRefreshTokenService;

    public RefreshTokenControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the refresh token service
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IRefreshTokenService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                _mockRefreshTokenService = new Mock<IRefreshTokenService>();
                services.AddScoped<IRefreshTokenService>(_ => _mockRefreshTokenService.Object);
            });
        });
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("valid-refresh-token");
        var userDto = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var authResponse = new AuthResponseDto("new-access-token", "new-refresh-token", DateTime.UtcNow.AddMinutes(60), userDto);

        _mockRefreshTokenService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync((true, "Token refreshed successfully.", authResponse));

        // Act
        var response = await client.PostAsJsonAsync("/api/RefreshToken/refresh", refreshTokenRequest);

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
    public async Task RefreshToken_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("invalid-refresh-token");

        _mockRefreshTokenService.Setup(x => x.RefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync((false, "Invalid refresh token.", null));

        // Act
        var response = await client.PostAsJsonAsync("/api/RefreshToken/refresh", refreshTokenRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Invalid refresh token.", apiResponse.Message);
    }

    [Fact]
    public async Task RefreshToken_EmptyToken_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("");

        // Act
        var response = await client.PostAsJsonAsync("/api/RefreshToken/refresh", refreshTokenRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RevokeToken_ValidToken_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("valid-refresh-token");

        _mockRefreshTokenService.Setup(x => x.RevokeRefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RefreshToken/revoke", refreshTokenRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Token revoked successfully", apiResponse.Message);
    }

    [Fact]
    public async Task RevokeToken_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var refreshTokenRequest = new RefreshTokenRequest("invalid-refresh-token");

        _mockRefreshTokenService.Setup(x => x.RevokeRefreshTokenAsync(refreshTokenRequest.RefreshToken))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RefreshToken/revoke", refreshTokenRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Token could not be revoked", apiResponse.Message);
    }
}
