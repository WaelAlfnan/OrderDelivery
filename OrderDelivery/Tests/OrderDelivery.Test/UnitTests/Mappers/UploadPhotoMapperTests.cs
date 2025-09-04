using Microsoft.AspNetCore.Http;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using Moq;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class UploadPhotoMapperTests
{
    [Fact]
    public void ToUploadPhotoResponseDto_ValidFile_ReturnsCorrectResponseDto()
    {
        // Arrange
        var mockFile = CreateMockFormFile("test-photo.jpg", "image/jpeg", 1024);
        var fileUrl = "https://s3.amazonaws.com/bucket/test-photo.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal("test-photo.jpg", result.FileName);
        Assert.Equal(1024, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithLongName_ReturnsCorrectResponseDto()
    {
        // Arrange
        var longFileName = "very-long-file-name-that-might-cause-issues-in-some-systems.jpg";
        var mockFile = CreateMockFormFile(longFileName, "image/jpeg", 2048);
        var fileUrl = "https://s3.amazonaws.com/bucket/very-long-file-name-that-might-cause-issues-in-some-systems.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal(longFileName, result.FileName);
        Assert.Equal(2048, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithSpecialCharacters_ReturnsCorrectResponseDto()
    {
        // Arrange
        var fileName = "صورة-الشخصية-123.jpg";
        var mockFile = CreateMockFormFile(fileName, "image/jpeg", 1536);
        var fileUrl = "https://s3.amazonaws.com/bucket/صورة-الشخصية-123.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal(fileName, result.FileName);
        Assert.Equal(1536, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithEmptyName_ReturnsCorrectResponseDto()
    {
        // Arrange
        var mockFile = CreateMockFormFile("", "image/png", 512);
        var fileUrl = "https://s3.amazonaws.com/bucket/empty-name.png";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal("", result.FileName);
        Assert.Equal(512, result.FileSize);
        Assert.Equal("image/png", result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithNullContentType_ReturnsCorrectResponseDto()
    {
        // Arrange
        var mockFile = CreateMockFormFile("test.jpg", null, 1024);
        var fileUrl = "https://s3.amazonaws.com/bucket/test.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal("test.jpg", result.FileName);
        Assert.Equal(1024, result.FileSize);
        Assert.Null(result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithZeroSize_ReturnsCorrectResponseDto()
    {
        // Arrange
        var mockFile = CreateMockFormFile("empty.jpg", "image/jpeg", 0);
        var fileUrl = "https://s3.amazonaws.com/bucket/empty.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal("empty.jpg", result.FileName);
        Assert.Equal(0, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    [Fact]
    public void ToUploadPhotoResponseDto_FileWithLargeSize_ReturnsCorrectResponseDto()
    {
        // Arrange
        var mockFile = CreateMockFormFile("large-photo.jpg", "image/jpeg", 10485760); // 10MB
        var fileUrl = "https://s3.amazonaws.com/bucket/large-photo.jpg";

        // Act
        var result = UploadPhotoMapper.ToUploadPhotoResponseDto(mockFile, fileUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileUrl, result.FileUrl);
        Assert.Equal("large-photo.jpg", result.FileName);
        Assert.Equal(10485760, result.FileSize);
        Assert.Equal("image/jpeg", result.ContentType);
    }

    private static IFormFile CreateMockFormFile(string fileName, string? contentType, long length)
    {
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.ContentType).Returns(contentType);
        mockFile.Setup(f => f.Length).Returns(length);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        return mockFile.Object;
    }
}
