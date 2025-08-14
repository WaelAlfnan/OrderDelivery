using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Services;
using Xunit;

namespace UnitTests.Application;

public class JwtServiceTests
{
    [Fact]
    public void GenerateAccessToken_ValidUserAndRoles_ReturnsTokenString()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            SecretKey = "supersecretkey1234567890supersecretkey1234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInMinutes = 60,
            RefreshTokenExpirationInDays = 7
        };
        var options = Options.Create(jwtSettings);
        var service = new JwtService(options);
        var user = new UserDto("1", "Test User", "0500000000", true, "User");
        var roles = new List<string> { "User" };
        // Act
        var token = service.GenerateAccessToken(user, roles);
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        // Arrange
        var jwtSettings = new JwtSettings();
        var options = Options.Create(jwtSettings);
        var service = new JwtService(options);
        // Act
        var refreshToken = service.GenerateRefreshToken();
        // Assert
        Assert.False(string.IsNullOrWhiteSpace(refreshToken));
    }
} 