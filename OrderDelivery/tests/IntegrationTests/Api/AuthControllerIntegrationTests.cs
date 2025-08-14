using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using Xunit;

namespace IntegrationTests.Api;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerDto = new RegisterDto("Integration Test", $"05{new Random().Next(10000000,99999999)}", "Test@123", "User");
        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(registerDto.PhoneNumber, apiResponse.Data!.PhoneNumber);
    }
} 