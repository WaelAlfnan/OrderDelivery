using OrderDelivery.Application.DTOs.RegistrationSteps.Requests;
using OrderDelivery.Application.Mappers;
using Xunit;

namespace OrderDelivery.UnitTests.Mappers;

public class BasicInfoMapperTests
{
    [Fact]
    public void ToBasicInfoData_ValidDto_ReturnsCorrectBasicInfoData()
    {
        // Arrange
        var dto = new SetBasicInfoDto(
            PhoneNumber: "+1234567890",
            FullName: "John Doe",
            PersonalPhotoUrl: "https://example.com/photo.jpg",
            NationalIdFrontPhotoUrl: "https://example.com/front.jpg",
            NationalIdBackPhotoUrl: "https://example.com/back.jpg",
            NationalIdNumber: "123456789"
        );

        // Act
        var result = BasicInfoMapper.ToBasicInfoData(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("https://example.com/photo.jpg", result.PersonalPhotoUrl);
        Assert.Equal("https://example.com/front.jpg", result.NationalIdFrontPhotoUrl);
        Assert.Equal("https://example.com/back.jpg", result.NationalIdBackPhotoUrl);
        Assert.Equal("123456789", result.NationalIdNumber);
    }

    [Fact]
    public void ToBasicInfoData_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        SetBasicInfoDto? dto = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => BasicInfoMapper.ToBasicInfoData(dto));
        Assert.Equal("dto", exception.ParamName);
    }

    [Fact]
    public void ToBasicInfoData_DtoWithNullValues_ReturnsEmptyStrings()
    {
        // Arrange
        var dto = new SetBasicInfoDto(
            PhoneNumber: "+1234567890",
            FullName: null!,
            PersonalPhotoUrl: null!,
            NationalIdFrontPhotoUrl: null!,
            NationalIdBackPhotoUrl: null!,
            NationalIdNumber: null!
        );

        // Act
        var result = BasicInfoMapper.ToBasicInfoData(dto);

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
        var result = BasicInfoMapper.ToSetBasicInfoDto(basicInfo, phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        Assert.Equal("Jane Smith", result.FullName);
        Assert.Equal("https://example.com/jane.jpg", result.PersonalPhotoUrl);
        Assert.Equal("https://example.com/jane-front.jpg", result.NationalIdFrontPhotoUrl);
        Assert.Equal("https://example.com/jane-back.jpg", result.NationalIdBackPhotoUrl);
        Assert.Equal("987654321", result.NationalIdNumber);
    }

    [Fact]
    public void ToSetBasicInfoDto_NullBasicInfoData_ThrowsArgumentNullException()
    {
        // Arrange
        BasicInfoData? basicInfo = null;
        var phoneNumber = "+1234567890";

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => BasicInfoMapper.ToSetBasicInfoDto(basicInfo, phoneNumber));
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
        var result = BasicInfoMapper.ToSetBasicInfoDto(basicInfo, phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        Assert.Equal(string.Empty, result.FullName);
        Assert.Equal(string.Empty, result.PersonalPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdFrontPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdBackPhotoUrl);
        Assert.Equal(string.Empty, result.NationalIdNumber);
    }
}
