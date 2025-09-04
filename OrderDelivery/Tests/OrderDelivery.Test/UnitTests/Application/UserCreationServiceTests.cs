using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OrderDelivery.Application.Services;
using OrderDelivery.Domain.Entities;
using Xunit;

namespace OrderDelivery.UnitTests.Application;

public class UserCreationServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _mockRoleManager;
    private readonly Mock<ILogger<UserCreationService>> _mockLogger;
    private readonly UserCreationService _userCreationService;

    public UserCreationServiceTests()
    {
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _mockRoleManager = new Mock<RoleManager<IdentityRole<Guid>>>(
            Mock.Of<IRoleStore<IdentityRole<Guid>>>(), null!, null!, null!, null!);
        _mockLogger = new Mock<ILogger<UserCreationService>>();

        _userCreationService = new UserCreationService(
            _mockUserManager.Object,
            _mockRoleManager.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ValidParameters_ReturnsSuccessAndUser()
    {
        // Arrange
        var userName = "+966501234567";
        var phoneNumber = "+966501234567";
        var password = "password123";
        var firstName = "أحمد";
        var lastName = "علي";
        var personalPhotoUrl = "https://example.com/photo.jpg";
        var nationalIdFrontPhotoUrl = "https://example.com/front.jpg";
        var nationalIdBackPhotoUrl = "https://example.com/back.jpg";
        var nationalIdNumber = "1234567890";

        var createdUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = phoneNumber,
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            PersonalPhotoUrl = personalPhotoUrl,
            NationalIdFrontPhotoUrl = nationalIdFrontPhotoUrl,
            NationalIdBackPhotoUrl = nationalIdBackPhotoUrl,
            NationalIdNumber = nationalIdNumber
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync((ApplicationUser)null!);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.True(success);
        Assert.Equal("User created successfully", message);
        Assert.NotNull(user);
        Assert.Equal(phoneNumber, user!.UserName);
        Assert.Equal(phoneNumber, user.PhoneNumber);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(personalPhotoUrl, user.PersonalPhotoUrl);
        Assert.Equal(nationalIdFrontPhotoUrl, user.NationalIdFrontPhotoUrl);
        Assert.Equal(nationalIdBackPhotoUrl, user.NationalIdBackPhotoUrl);
        Assert.Equal(nationalIdNumber, user.NationalIdNumber);
    }

    [Fact]
    public async Task CreateUserAsync_UserAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var userName = "+966501234567";
        var phoneNumber = "+966501234567";
        var password = "password123";
        var firstName = "أحمد";
        var lastName = "علي";
        var personalPhotoUrl = "https://example.com/photo.jpg";
        var nationalIdFrontPhotoUrl = "https://example.com/front.jpg";
        var nationalIdBackPhotoUrl = "https://example.com/back.jpg";
        var nationalIdNumber = "1234567890";

        var existingUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = phoneNumber,
            PhoneNumber = phoneNumber
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync(existingUser);

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.False(success);
        Assert.Equal("User with this phone number already exists", message);
        Assert.Null(user);
    }

    [Fact]
    public async Task CreateUserAsync_CreationFails_ReturnsFailure()
    {
        // Arrange
        var userName = "+966501234567";
        var phoneNumber = "+966501234567";
        var password = "weak";
        var firstName = "أحمد";
        var lastName = "علي";
        var personalPhotoUrl = "https://example.com/photo.jpg";
        var nationalIdFrontPhotoUrl = "https://example.com/front.jpg";
        var nationalIdBackPhotoUrl = "https://example.com/back.jpg";
        var nationalIdNumber = "1234567890";

        var identityErrors = new List<IdentityError>
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
        };

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync((ApplicationUser)null!);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.False(success);
        Assert.Contains("PasswordTooShort", message);
        Assert.Null(user);
    }

    [Fact]
    public async Task CreateUserAsync_ExceptionThrown_ReturnsFailure()
    {
        // Arrange
        var userName = "+966501234567";
        var phoneNumber = "+966501234567";
        var password = "password123";
        var firstName = "أحمد";
        var lastName = "علي";
        var personalPhotoUrl = "https://example.com/photo.jpg";
        var nationalIdFrontPhotoUrl = "https://example.com/front.jpg";
        var nationalIdBackPhotoUrl = "https://example.com/back.jpg";
        var nationalIdNumber = "1234567890";

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.False(success);
        Assert.Equal("An error occurred while creating the user", message);
        Assert.Null(user);
    }

    [Fact]
    public async Task CreateUserAsync_EmptyParameters_ReturnsSuccessWithEmptyValues()
    {
        // Arrange
        var userName = "";
        var phoneNumber = "";
        var password = "password123";
        var firstName = "";
        var lastName = "";
        var personalPhotoUrl = "";
        var nationalIdFrontPhotoUrl = "";
        var nationalIdBackPhotoUrl = "";
        var nationalIdNumber = "";

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync((ApplicationUser)null!);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.True(success);
        Assert.Equal("User created successfully", message);
        Assert.NotNull(user);
        Assert.Equal("", user!.UserName);
        Assert.Equal("", user.PhoneNumber);
        Assert.Equal("", user.FirstName);
        Assert.Equal("", user.LastName);
        Assert.Equal("", user.PersonalPhotoUrl);
        Assert.Equal("", user.NationalIdFrontPhotoUrl);
        Assert.Equal("", user.NationalIdBackPhotoUrl);
        Assert.Equal("", user.NationalIdNumber);
    }

    [Fact]
    public async Task CreateUserAsync_NullParameters_ReturnsSuccessWithEmptyValues()
    {
        // Arrange
        string? userName = null;
        string? phoneNumber = null;
        var password = "password123";
        string? firstName = null;
        string? lastName = null;
        string? personalPhotoUrl = null;
        string? nationalIdFrontPhotoUrl = null;
        string? nationalIdBackPhotoUrl = null;
        string? nationalIdNumber = null;

        _mockUserManager.Setup(x => x.FindByNameAsync(phoneNumber))
            .ReturnsAsync((ApplicationUser)null!);
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var (success, message, user) = await _userCreationService.CreateUserAsync(
            userName, phoneNumber, password, firstName, lastName,
            personalPhotoUrl, nationalIdFrontPhotoUrl, nationalIdBackPhotoUrl, nationalIdNumber);

        // Assert
        Assert.True(success);
        Assert.Equal("User created successfully", message);
        Assert.NotNull(user);
        Assert.Null(user!.UserName);
        Assert.Null(user.PhoneNumber);
        Assert.Null(user.FirstName);
        Assert.Null(user.LastName);
        Assert.Null(user.PersonalPhotoUrl);
        Assert.Null(user.NationalIdFrontPhotoUrl);
        Assert.Null(user.NationalIdBackPhotoUrl);
        Assert.Null(user.NationalIdNumber);
    }
}
