using Microsoft.Extensions.Options;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace OrderDelivery.UnitTests.Application;

public class JwtServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsAVeryLongSecretKeyForTestingPurposesOnly123456789",
            Issuer = "OrderDelivery",
            Audience = "OrderDeliveryUsers",
            ExpirationInMinutes = 60
        };

        var options = Options.Create(_jwtSettings);
        _jwtService = new JwtService(options);
    }

    [Fact]
    public void GenerateAccessToken_ValidUserAndRoles_ReturnsValidToken()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var roles = new List<string> { "Driver" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify token can be parsed
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        Assert.Equal(_jwtSettings.Issuer, jwtToken.Issuer);
        Assert.Equal(_jwtSettings.Audience, jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.FullName);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.MobilePhone && c.Value == user.PhoneNumber);
        Assert.Contains(jwtToken.Claims, c => c.Type == "PhoneConfirmed" && c.Value == "True");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Driver");
    }

    [Fact]
    public void GenerateAccessToken_UserWithMultipleRoles_ReturnsTokenWithAllRoles()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var roles = new List<string> { "Driver", "Admin" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains(roleClaims, c => c.Value == "Driver");
        Assert.Contains(roleClaims, c => c.Value == "Admin");
    }

    [Fact]
    public void GenerateAccessToken_UserWithNoRoles_ReturnsTokenWithoutRoleClaims()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "");
        var roles = new List<string>();

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        Assert.Empty(roleClaims);
    }

    [Fact]
    public void GenerateAccessToken_UserWithEmptyName_ReturnsTokenWithEmptyName()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "", "+966501234567", true, "Driver");
        var roles = new List<string> { "Driver" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        Assert.NotNull(nameClaim);
        Assert.Equal("", nameClaim.Value);
    }

    [Fact]
    public void GenerateAccessToken_UserWithUnconfirmedPhone_ReturnsTokenWithPhoneConfirmedFalse()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", false, "Driver");
        var roles = new List<string> { "Driver" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var phoneConfirmedClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "PhoneConfirmed");
        Assert.NotNull(phoneConfirmedClaim);
        Assert.Equal("False", phoneConfirmedClaim.Value);
    }

    [Fact]
    public void GenerateAccessToken_TokenExpiration_IsSetCorrectly()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var roles = new List<string> { "Driver" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        var actualExpiration = jwtToken.ValidTo;
        
        // Allow for small time differences (within 1 minute)
        Assert.True(Math.Abs((expectedExpiration - actualExpiration).TotalMinutes) < 1);
    }

    [Fact]
    public void GenerateAccessToken_TokenContainsJtiAndIat_ReturnsTokenWithStandardClaims()
    {
        // Arrange
        var user = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var roles = new List<string> { "Driver" };

        // Act
        var token = _jwtService.GenerateAccessToken("1", user, roles);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        var iatClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat);
        
        Assert.NotNull(jtiClaim);
        Assert.NotNull(iatClaim);
        Assert.True(Guid.TryParse(jtiClaim.Value, out _));
        Assert.True(long.TryParse(iatClaim.Value, out _));
    }

    [Fact]
    public void GenerateAccessToken_DifferentUsers_GenerateDifferentTokens()
    {
        // Arrange
        var user1 = new UserDto(Guid.NewGuid(), "أحمد علي", "+966501234567", true, "Driver");
        var user2 = new UserDto(Guid.NewGuid(), "محمد أحمد", "+966501234568", true, "Merchant");
        var roles = new List<string> { "Driver" };

        // Act
        var token1 = _jwtService.GenerateAccessToken("1", user1, roles);
        var token2 = _jwtService.GenerateAccessToken("2", user2, roles);

        // Assert
        Assert.NotEqual(token1, token2);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken1 = tokenHandler.ReadJwtToken(token1);
        var jwtToken2 = tokenHandler.ReadJwtToken(token2);
        
        Assert.NotEqual(jwtToken1.Id, jwtToken2.Id); // Different JTI
    }
}