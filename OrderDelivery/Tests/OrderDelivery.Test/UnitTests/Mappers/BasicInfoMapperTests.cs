using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class BasicInfoMapperTests
{
    [Fact]
    public void ToBasicInfoData_ValidDto_ReturnsCorrectBasicInfoData()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal-photo.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front-photo.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back-photo.jpg", "image/jpeg", 1024);
        
        var dto = new SetBasicInfoDto(
            PhoneNumber: "+1234567890",
            FullName: "John Doe",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "123456789"
        );

        // Act
        var result = dto.ToBasicInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal(string.Empty, result.PersonalPhotoUrl); // Will be set by service after S3 upload
        Assert.Equal(string.Empty, result.NationalIdFrontPhotoUrl); // Will be set by service after S3 upload
        Assert.Equal(string.Empty, result.NationalIdBackPhotoUrl); // Will be set by service after S3 upload
        Assert.Equal("123456789", result.NationalIdNumber);
    }

    [Fact]
    public void ToBasicInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        SetBasicInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => dto!.ToBasicInfoData());
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToBasicInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var mockPersonalPhoto = CreateMockFormFile("personal-photo.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front-photo.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back-photo.jpg", "image/jpeg", 1024);
        
        var dto = new SetBasicInfoDto(
            PhoneNumber: "+1234567890",
            FullName: null!,
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: null!
        );

        // Act
        var result = dto.ToBasicInfoData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.FullName);
        Assert.Equal(string.Empty, result.PersonalPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdFrontPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdBackPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdNumber);
    }

    [Fact]
    public void ToSetBasicInfoDto_ValidBasicInfoData_ReturnsCorrectDto()
    {
        // Arrange
        var basicInfo = new BasicInfoData
        {
            FullName = "Jane Smith",
            PersonalPhotoUrl = "https://example.com/jane.jpg",
            NationalIdFrontPhotoUrl = "https://example.com/jane-front.jpg",
            NationalIdBackPhotoUrl = "https://example.com/jane-back.jpg",
            NationalIdNumber = "987654321"
        };
        var phoneNumber = "+9876543210";

        // Act
        var result = basicInfo.ToSetBasicInfoDto(phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        Assert.Equal("Jane Smith", result.FullName);
        Assert.Null(result.PersonalPhoto); // Photos are not included in DTO for updates
        Assert.Null(result.NationalIdFrontPhoto); // Photos are not included in DTO for updates
        Assert.Null(result.NationalIdBackPhoto); // Photos are not included in DTO for updates
        Assert.Equal("987654321", result.NationalIdNumber);
    }

    [Fact]
    public void ToSetBasicInfoDto_NullBasicInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        BasicInfoData? basicInfo = null;
        var phoneNumber = "+1234567890";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => basicInfo!.ToSetBasicInfoDto(phoneNumber));
        Assert.Equal("basicInfo", exception.ParamName);
    }

    [Fact]
    public void ToSetBasicInfoDto_BasicInfoDataWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var basicInfo = new BasicInfoData
        {
            FullName = null!,
            PersonalPhotoUrl = null!,
            NationalIdFrontPhotoUrl = null!,
            NationalIdBackPhotoUrl = null!,
            NationalIdNumber = null!
        };
        var phoneNumber = "+1234567890";

        // Act
        var result = basicInfo.ToSetBasicInfoDto(phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        Assert.Equal(string.Empty, result.FullName);
        Assert.Null(result.PersonalPhoto); // Photos are not included in DTO for updates
        Assert.Null(result.NationalIdFrontPhoto); // Photos are not included in DTO for updates
        Assert.Null(result.NationalIdBackPhoto); // Photos are not included in DTO for updates
        Assert.Equal(string.Empty, result.NationalIdNumber);
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
