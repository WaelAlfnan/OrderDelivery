using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Api;
using Xunit;

namespace IntegrationTests.Api;

public class RegistrationStepsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<IRegistrationStepsService> _mockRegistrationStepsService;

    public RegistrationStepsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the registration steps service
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IRegistrationStepsService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                _mockRegistrationStepsService = new Mock<IRegistrationStepsService>();
                services.AddScoped<IRegistrationStepsService>(_ => _mockRegistrationStepsService.Object);
            });
        });
    }

    [Fact]
    public async Task StartRegistration_ValidPhoneNumber_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerPhoneDto = new RegisterPhoneDto("+966501234567");

        _mockRegistrationStepsService.Setup(x => x.StartRegistrationAsync(registerPhoneDto))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/start-registration", registerPhoneDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Registration started successfully", apiResponse.Message);
    }

    [Fact]
    public async Task StartRegistration_PhoneNumberAlreadyRegistered_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerPhoneDto = new RegisterPhoneDto("+966501234567");

        _mockRegistrationStepsService.Setup(x => x.StartRegistrationAsync(registerPhoneDto))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/start-registration", registerPhoneDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Phone number already registered", apiResponse.Message);
    }

    [Fact]
    public async Task StartRegistration_EmptyPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerPhoneDto = new RegisterPhoneDto("");

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/start-registration", registerPhoneDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VerifyPhone_ValidCode_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var verifyPhoneDto = new VerifyPhoneDto("+966501234567", "123456");

        _mockRegistrationStepsService.Setup(x => x.VerifyPhoneAsync(verifyPhoneDto))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/verify-phone", verifyPhoneDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Phone verified successfully", apiResponse.Message);
    }

    [Fact]
    public async Task VerifyPhone_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var verifyPhoneDto = new VerifyPhoneDto("+966501234567", "000000");

        _mockRegistrationStepsService.Setup(x => x.VerifyPhoneAsync(verifyPhoneDto))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/verify-phone", verifyPhoneDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Invalid verification code", apiResponse.Message);
    }

    [Fact]
    public async Task SetRole_ValidRole_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var setRoleDto = new SetRoleDto("+966501234567", "Driver");

        _mockRegistrationStepsService.Setup(x => x.SetRoleAsync(setRoleDto))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/set-role", setRoleDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Role set successfully", apiResponse.Message);
    }

    [Fact]
    public async Task SetRole_InvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var setRoleDto = new SetRoleDto("+966501234567", "InvalidRole");

        _mockRegistrationStepsService.Setup(x => x.SetRoleAsync(setRoleDto))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/set-role", setRoleDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Invalid role", apiResponse.Message);
    }

    [Fact]
    public async Task SetPassword_ValidPassword_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var setPasswordDto = new SetPasswordDto("+966501234567", "password123");

        _mockRegistrationStepsService.Setup(x => x.SetPasswordAsync(setPasswordDto))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/set-password", setPasswordDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Password set successfully", apiResponse.Message);
    }

    [Fact]
    public async Task SetPassword_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var setPasswordDto = new SetPasswordDto("+966501234567", "123");

        _mockRegistrationStepsService.Setup(x => x.SetPasswordAsync(setPasswordDto))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/set-password", setPasswordDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Password does not meet requirements", apiResponse.Message);
    }

    [Fact]
    public async Task SetBasicInfo_ValidData_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var mockPersonalPhoto = CreateMockFormFile("personal.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back.jpg", "image/jpeg", 1024);

        var formData = new MultipartFormDataContent
        {
            { new StringContent("+966501234567"), "PhoneNumber" },
            { new StringContent("أحمد علي"), "FullName" },
            { new StringContent("1234567890"), "NationalIdNumber" },
            { new StreamContent(mockPersonalPhoto.OpenReadStream()), "PersonalPhoto", "personal.jpg" },
            { new StreamContent(mockFrontPhoto.OpenReadStream()), "NationalIdFrontPhoto", "front.jpg" },
            { new StreamContent(mockBackPhoto.OpenReadStream()), "NationalIdBackPhoto", "back.jpg" }
        };

        _mockRegistrationStepsService.Setup(x => x.SetBasicInfoAsync(It.IsAny<SetBasicInfoDto>()))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsync("/api/RegistrationSteps/set-basic-info", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Basic info set successfully", apiResponse.Message);
    }

    [Fact]
    public async Task SetBasicInfo_MissingPhotos_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var formData = new MultipartFormDataContent
        {
            { new StringContent("+966501234567"), "PhoneNumber" },
            { new StringContent("أحمد علي"), "FullName" },
            { new StringContent("1234567890"), "NationalIdNumber" }
        };

        // Act
        var response = await client.PostAsync("/api/RegistrationSteps/set-basic-info", formData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CompleteRegistration_ValidData_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var completeRegistrationDto = new CompleteRegistrationDto("+966501234567");

        _mockRegistrationStepsService.Setup(x => x.CompleteRegistrationAsync(completeRegistrationDto.PhoneNumber))
            .ReturnsAsync(true);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/complete-registration", completeRegistrationDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.Equal("Registration completed successfully", apiResponse.Message);
    }

    [Fact]
    public async Task CompleteRegistration_InvalidPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var completeRegistrationDto = new CompleteRegistrationDto("+966501234567");

        _mockRegistrationStepsService.Setup(x => x.CompleteRegistrationAsync(completeRegistrationDto.PhoneNumber))
            .ReturnsAsync(false);

        // Act
        var response = await client.PostAsJsonAsync("/api/RegistrationSteps/complete-registration", completeRegistrationDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        Assert.NotNull(apiResponse);
        Assert.False(apiResponse!.Success);
        Assert.Equal("Registration could not be completed", apiResponse.Message);
    }

    private static IFormFile CreateMockFormFile(string fileName, string contentType, long length)
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        return mockFile.Object;
    }
}
