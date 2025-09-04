using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Api;
using Xunit;

namespace IntegrationTests.Api;

public class FileUploadControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private Mock<IFileStorageService> _mockFileStorageService;

    public FileUploadControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the file storage service
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IFileStorageService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                _mockFileStorageService = new Mock<IFileStorageService>();
                services.AddScoped<IFileStorageService>(_ => _mockFileStorageService.Object);
            });
        });
    }

    [Fact]
    public async Task UploadPhoto_ValidFile_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test-photo.jpg");
        var uploadPhotoDto = new UploadPhotoDto(mockFile.Object);

        _mockFileStorageService.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://s3.amazonaws.com/bucket/test-photo.jpg");

        // Act
        var response = await client.PostAsJsonAsync("/api/FileUpload/upload-photo", uploadPhotoDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UploadPhotoResponseDto>>();
        Assert.NotNull(apiResponse);
        Assert.True(apiResponse!.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("https://s3.amazonaws.com/bucket/test-photo.jpg", apiResponse.Data!.FileUrl);
    }

    [Fact]
    public async Task UploadPhoto_InvalidFile_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("");
        var uploadPhotoDto = new UploadPhotoDto(mockFile.Object);

        // Act
        var response = await client.PostAsJsonAsync("/api/FileUpload/upload-photo", uploadPhotoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadPhoto_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var client = _factory.CreateClient();
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test-photo.jpg");
        var uploadPhotoDto = new UploadPhotoDto(mockFile.Object);

        _mockFileStorageService.Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Upload failed"));

        // Act
        var response = await client.PostAsJsonAsync("/api/FileUpload/upload-photo", uploadPhotoDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
