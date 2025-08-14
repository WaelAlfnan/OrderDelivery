using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace OrderDelivery.Tests.IntegrationTests.Api;

public class FileUploadControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IFileStorageService> _mockFileStorageService;

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
    public async Task UploadPhoto_ValidFile_ReturnsSuccessResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var expectedResponse = new UploadPhotoResponseDto(
            "https://test-bucket.s3.amazonaws.com/test.jpg",
            "test.jpg",
            1024,
            "image/jpeg"
        );

        _mockFileStorageService
            .Setup(x => x.UploadPhotoAsync(It.IsAny<OrderDelivery.Application.DTOs.Requests.UploadPhotoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Create test file content
        var fileContent = "test image content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Create multipart form data
        using var formData = new MultipartFormDataContent();
        var fileContent2 = new StreamContent(stream);
        fileContent2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent2, "File", "test.jpg");

        // Act
        var response = await client.PostAsync("/api/fileupload/upload-photo", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Photo uploaded successfully", responseContent);
    }

    [Fact]
    public async Task UploadPhoto_InvalidFile_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create multipart form data without file
        using var formData = new MultipartFormDataContent();

        // Act
        var response = await client.PostAsync("/api/fileupload/upload-photo", formData);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadPhotoWithFolder_ValidFileAndFolder_ReturnsSuccessResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var expectedResponse = new UploadPhotoResponseDto(
            "https://test-bucket.s3.amazonaws.com/users/photos/test.jpg",
            "test.jpg",
            1024,
            "image/jpeg"
        );

        _mockFileStorageService
            .Setup(x => x.UploadPhotoAsync(It.IsAny<OrderDelivery.Application.DTOs.Requests.UploadPhotoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Create test file content
        var fileContent = "test image content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Create multipart form data
        using var formData = new MultipartFormDataContent();
        var fileContent2 = new StreamContent(stream);
        fileContent2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        formData.Add(fileContent2, "File", "test.jpg");
        formData.Add(new StringContent("users/photos"), "FolderName");

        // Act
        var response = await client.PostAsync("/api/fileupload/upload-photo-with-folder", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Photo uploaded successfully", responseContent);
    }

    [Fact]
    public async Task DeletePhoto_ValidUrl_ReturnsSuccessResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var fileUrl = "https://test-bucket.s3.amazonaws.com/test.jpg";

        _mockFileStorageService
            .Setup(x => x.DeletePhotoAsync(fileUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var response = await client.DeleteAsync($"/api/fileupload/delete-photo?fileUrl={Uri.EscapeDataString(fileUrl)}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Photo deleted successfully", responseContent);
    }

    [Fact]
    public async Task CheckPhotoExists_ExistingPhoto_ReturnsTrue()
    {
        // Arrange
        var client = _factory.CreateClient();
        var fileUrl = "https://test-bucket.s3.amazonaws.com/test.jpg";

        _mockFileStorageService
            .Setup(x => x.PhotoExistsAsync(fileUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var response = await client.GetAsync($"/api/fileupload/check-photo-exists?fileUrl={Uri.EscapeDataString(fileUrl)}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"Exists\":true", responseContent);
    }
}
