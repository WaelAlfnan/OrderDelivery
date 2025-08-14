using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers;
using OrderDelivery.Application.Services;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace UnitTests.Application;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsSuccessAndUserDto()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManagerMock.Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);
        var jwtServiceMock = new Mock<IJwtService>();
        var smsServiceMock = new Mock<ISmsService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null!);
        userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        smsServiceMock.Setup(x => x.SendVerificationCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var service = new AuthService(
            userManagerMock.Object,
            signInManagerMock.Object,
            jwtServiceMock.Object,
            smsServiceMock.Object,
            loggerMock.Object);

        var registerDto = new RegisterDto("أحمد علي", "0500000000", "pass123", "User");

        // Act
        var (success, message, userDto) = await service.RegisterAsync(registerDto);

        // Assert
        Assert.True(success);
        Assert.NotNull(userDto);
        Assert.Equal(registerDto.PhoneNumber, userDto!.PhoneNumber);
        Assert.Equal(registerDto.FullName, userDto.FullName);
        Assert.Equal(registerDto.Role, userDto.Role);
        Assert.False(userDto.IsPhoneConfirmed);
    }
} 