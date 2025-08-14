using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Http;
using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Services;
using OrderDelivery.Domain;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Domain.Enums;
using System.Linq.Expressions;
using System.Text.Json;
using Xunit;

namespace OrderDelivery.UnitTests.Application;

public class RegistrationStepsServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserCreationService> _mockUserCreationService;
    private readonly Mock<ISmsService> _mockSmsService;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    private readonly Mock<ILogger<RegistrationStepsService>> _mockLogger;
    private readonly Mock<IGenericRepository<PendingRegistration>> _mockPendingRepo;
    private readonly RegistrationStepsService _service;

    public RegistrationStepsServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserCreationService = new Mock<IUserCreationService>();
        _mockSmsService = new Mock<ISmsService>();
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockLogger = new Mock<ILogger<RegistrationStepsService>>();
        _mockPendingRepo = new Mock<IGenericRepository<PendingRegistration>>();

        _mockUnitOfWork.Setup(uow => uow.Repository<PendingRegistration>())
            .Returns(_mockPendingRepo.Object);

        _service = new RegistrationStepsService(
            _mockUnitOfWork.Object,
            _mockUserCreationService.Object,
            _mockSmsService.Object,
            _mockFileStorageService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task SetBasicInfoAsync_WithPhotos_ShouldUploadToS3AndSaveData()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var pending = new PendingRegistration
        {
            PhoneNumber = phoneNumber,
            Step = 4,
            IsPhoneVerified = true
        };

        var mockPersonalPhoto = CreateMockFormFile("personal-photo.jpg", "image/jpeg", 1024);
        var mockFrontPhoto = CreateMockFormFile("front-photo.jpg", "image/jpeg", 1024);
        var mockBackPhoto = CreateMockFormFile("back-photo.jpg", "image/jpeg", 1024);

        var dto = new SetBasicInfoDto(
            PhoneNumber: phoneNumber,
            FullName: "John Doe",
            PersonalPhoto: mockPersonalPhoto,
            NationalIdFrontPhoto: mockFrontPhoto,
            NationalIdBackPhoto: mockBackPhoto,
            NationalIdNumber: "123456789"
        );

        _mockPendingRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PendingRegistration, bool>>>()))
            .ReturnsAsync(pending);

        _mockFileStorageService.Setup(fs => fs.UploadPhotoAsync(It.IsAny<UploadPhotoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadPhotoResponseDto("https://s3.amazonaws.com/bucket/photo.jpg", "photo.jpg", 1024, "image/jpeg"));

        // Act
        var result = await _service.SetBasicInfoAsync(dto);

        // Assert
        Assert.True(result);
        _mockFileStorageService.Verify(fs => fs.UploadPhotoAsync(It.IsAny<UploadPhotoDto>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetVehicleInfoAsync_WithPhotos_ShouldUploadToS3AndSaveData()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var pending = new PendingRegistration
        {
            PhoneNumber = phoneNumber,
            Step = 6,
            IsPhoneVerified = true,
            Role = "Driver"
        };

        var mockShasehPhoto = CreateMockFormFile("shaseh-photo.jpg", "image/jpeg", 1024);
        var mockEfragPhoto = CreateMockFormFile("efrag-photo.jpg", "image/jpeg", 1024);

        var dto = new VehicleInfoDto(
            PhoneNumber: phoneNumber,
            VehicleBrand: "Toyota",
            VehiclePlateNumber: "ABC123",
            VehicleIssueDate: DateTime.Now,
            ShasehPhoto: mockShasehPhoto,
            EfragPhoto: mockEfragPhoto
        );

        _mockPendingRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PendingRegistration, bool>>>()))
            .ReturnsAsync(pending);

        _mockFileStorageService.Setup(fs => fs.UploadPhotoAsync(It.IsAny<UploadPhotoDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadPhotoResponseDto("https://s3.amazonaws.com/bucket/photo.jpg", "photo.jpg", 1024, "image/jpeg"));

        // Act
        var result = await _service.SetVehicleInfoAsync(dto);

        // Assert
        Assert.True(result);
        _mockFileStorageService.Verify(fs => fs.UploadPhotoAsync(It.IsAny<UploadPhotoDto>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetBasicInfoAsync_WithoutPhotos_ShouldThrowException()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var pending = new PendingRegistration
        {
            PhoneNumber = phoneNumber,
            Step = 4,
            IsPhoneVerified = true
        };

        var dto = new SetBasicInfoDto(
            PhoneNumber: phoneNumber,
            FullName: "John Doe",
            PersonalPhoto: null!,
            NationalIdFrontPhoto: null!,
            NationalIdBackPhoto: null!,
            NationalIdNumber: "123456789"
        );

        _mockPendingRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PendingRegistration, bool>>>()))
            .ReturnsAsync(pending);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetBasicInfoAsync(dto));
    }

    [Fact]
    public async Task SetBasicInfoAsync_WithEmptyPhoto_ShouldThrowException()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var pending = new PendingRegistration
        {
            PhoneNumber = phoneNumber,
            Step = 4,
            IsPhoneVerified = true
        };

        var mockEmptyPhoto = CreateMockFormFile("empty-photo.jpg", "image/jpeg", 0); // Empty file

        var dto = new SetBasicInfoDto(
            PhoneNumber: phoneNumber,
            FullName: "John Doe",
            PersonalPhoto: mockEmptyPhoto,
            NationalIdFrontPhoto: mockEmptyPhoto,
            NationalIdBackPhoto: mockEmptyPhoto,
            NationalIdNumber: "123456789"
        );

        _mockPendingRepo.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<PendingRegistration, bool>>>()))
            .ReturnsAsync(pending);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SetBasicInfoAsync(dto));
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
