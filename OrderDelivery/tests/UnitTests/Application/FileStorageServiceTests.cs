using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Services;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace OrderDelivery.Tests.UnitTests.Application;

public class FileStorageServiceTests
{
    private readonly Mock<IAmazonS3> _mockS3Client;
    private readonly Mock<IOptions<S3Settings>> _mockS3Settings;
    private readonly Mock<ILogger<S3FileStorageService>> _mockLogger;
    private readonly S3Settings _s3Settings;
    private readonly S3FileStorageService _service;

    public FileStorageServiceTests()
    {
        _mockS3Client = new Mock<IAmazonS3>();
        _mockS3Settings = new Mock<IOptions<S3Settings>>();
        _mockLogger = new Mock<ILogger<S3FileStorageService>>();

        _s3Settings = new S3Settings
        {
            AccessKey = "test-access-key",
            SecretKey = "test-secret-key",
            BucketName = "test-bucket",
            Region = "us-east-1",
            BaseUrl = "https://test-bucket.s3.us-east-1.amazonaws.com"
        };

        _mockS3Settings.Setup(x => x.Value).Returns(_s3Settings);
        _service = new S3FileStorageService(_mockS3Client.Object, _mockS3Settings.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task UploadPhotoAsync_ValidFile_ReturnsUploadResponse()
    {
        // Arrange
        var fileContent = "test image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

        var uploadDto = new UploadPhotoDto(mockFile.Object, "test-folder");

        // Act
        var result = await _service.UploadPhotoAsync(uploadDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.jpg", result.FileName);
        Assert.Equal(stream.Length, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
        Assert.Contains("test-folder", result.FileUrl);
    }

    [Fact]
    public async Task UploadPhotoAsync_NoFolder_ReturnsUploadResponseWithRootPath()
    {
        // Arrange
        var fileContent = "test image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        mockFile.Setup(f => f.Length).Returns(stream.Length);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.OpenReadStream()).Returns(stream);

        var uploadDto = new UploadPhotoDto(mockFile.Object, null);

        // Act
        var result = await _service.UploadPhotoAsync(uploadDto);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("/", result.FileUrl.Replace(_s3Settings.BaseUrl, ""));
    }

    [Fact]
    public async Task DeletePhotoAsync_ValidUrl_ReturnsTrue()
    {
        // Arrange
        var fileUrl = $"{_s3Settings.BaseUrl}/test-folder/test.jpg";
        _mockS3Client.Setup(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse());

        // Act
        var result = await _service.DeletePhotoAsync(fileUrl);

        // Assert
        Assert.True(result);
        _mockS3Client.Verify(x => x.DeleteObjectAsync(It.IsAny<DeleteObjectRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PhotoExistsAsync_ExistingPhoto_ReturnsTrue()
    {
        // Arrange
        var fileUrl = $"{_s3Settings.BaseUrl}/test-folder/test.jpg";
        _mockS3Client.Setup(x => x.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectMetadataResponse());

        // Act
        var result = await _service.PhotoExistsAsync(fileUrl);

        // Assert
        Assert.True(result);
        _mockS3Client.Verify(x => x.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task PhotoExistsAsync_NonExistingPhoto_ReturnsFalse()
    {
        // Arrange
        var fileUrl = $"{_s3Settings.BaseUrl}/test-folder/test.jpg";
        _mockS3Client.Setup(x => x.GetObjectMetadataAsync(It.IsAny<GetObjectMetadataRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonS3Exception("Not found") { StatusCode = System.Net.HttpStatusCode.NotFound });

        // Act
        var result = await _service.PhotoExistsAsync(fileUrl);

        // Assert
        Assert.False(result);
    }
}
