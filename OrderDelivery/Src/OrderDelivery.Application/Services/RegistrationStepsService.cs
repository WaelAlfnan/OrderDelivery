using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrderDelivery.Application.DataModels.RegistrationSteps;
using OrderDelivery.Application.DTOs.RegistrationSteps;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;
using OrderDelivery.Domain;
using OrderDelivery.Domain.Entities;
using System.Security.Cryptography;
using System.Text.Json;

namespace OrderDelivery.Application.Services;

public class RegistrationStepsService : IRegistrationStepsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserCreationService _userCreationService;
    private readonly ISmsService _smsService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<RegistrationStepsService> _logger;

    // In-memory storage for verification codes (in production, use Redis or database)
    private static readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationCodes = new();

    public RegistrationStepsService(
        IUnitOfWork unitOfWork,
        IUserCreationService userCreationService,
        ISmsService smsService,
        IFileStorageService fileStorageService,
        ILogger<RegistrationStepsService> logger)
    {
        _unitOfWork = unitOfWork;
        _userCreationService = userCreationService;
        _smsService = smsService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<bool> StartRegistrationAsync(RegisterPhoneDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var exists = await repo.AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (exists) return false;

        var pending = new PendingRegistration
        {
            PhoneNumber = dto.PhoneNumber,
            Step = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await repo.AddAsync(pending);
        await _unitOfWork.SaveChangesAsync();

        // Generate and send verification code
        var verificationCode = GenerateVerificationCode();
        _verificationCodes[dto.PhoneNumber] = (verificationCode, DateTime.UtcNow.AddMinutes(10));

        var smsSent = await _smsService.SendVerificationCodeAsync(dto.PhoneNumber, verificationCode);
        if (!smsSent)
        {
            repo.Remove(pending);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogWarning("Failed to send verification SMS to {PhoneNumber}", dto.PhoneNumber);
            return false;
        }

        return true;
    }

    public async Task<bool> VerifyPhoneAsync(VerifyPhoneDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null) return false;

        // Check if verification code exists and is valid
        if (!_verificationCodes.TryGetValue(dto.PhoneNumber, out var codeInfo))
        {
            return false;
        }

        if (DateTime.UtcNow > codeInfo.Expiry)
        {
            _verificationCodes.Remove(dto.PhoneNumber);
            return false;
        }

        if (codeInfo.Code != dto.VerificationCode)
        {
            return false;
        }

        pending.IsPhoneVerified = true;
        pending.Step = 2;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();

        // Remove verification code
        _verificationCodes.Remove(dto.PhoneNumber);

        return true;
    }

    public async Task<bool> SetPasswordAsync(SetPasswordDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || !pending.IsPhoneVerified) return false;

        pending.Password = dto.Password;
        pending.Step = 3;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetRoleAsync(SetRoleDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 3) return false;

        pending.Role = dto.Role;
        pending.Step = 4;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetBasicInfoAsync(SetBasicInfoDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 4) return false;

        try
        {
            // Upload photos to S3 before saving data
            var personalPhotoUrl = await UploadPhotoToS3Async(dto.PersonalPhoto, "personal-photos");
            var nationalIdFrontPhotoUrl = await UploadPhotoToS3Async(dto.NationalIdFrontPhoto, "national-id-front");
            var nationalIdBackPhotoUrl = await UploadPhotoToS3Async(dto.NationalIdBackPhoto, "national-id-back");

            // Use mapper to convert DTO to BasicInfoData
            var basicInfo = dto.ToBasicInfoData();

            // Update photo URLs with S3 URLs
            basicInfo.PersonalPhotoUrl = personalPhotoUrl;
            basicInfo.NationalIdFrontPhotoUrl = nationalIdFrontPhotoUrl;
            basicInfo.NationalIdBackPhotoUrl = nationalIdBackPhotoUrl;

            pending.BasicInfoJson = JsonSerializer.Serialize(basicInfo);
            pending.Step = 5;
            pending.UpdatedAt = DateTime.UtcNow;
            repo.Update(pending);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Basic info set successfully for phone: {PhoneNumber}", dto.PhoneNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting basic info for phone: {PhoneNumber}", dto.PhoneNumber);
            return false;
        }
    }

    public async Task<bool> SetMerchantInfoAsync(MerchantInfoDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 5 || pending.Role != "Merchant") return false;

        var merchantInfo = dto.ToMerchantInfoData();
        pending.MerchantInfoJson = JsonSerializer.Serialize(merchantInfo);
        pending.Step = 6;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetDriverInfoAsync(DriverInfoDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 5 || pending.Role != "Driver") return false;

        var driverInfo = dto.ToDriverInfoData();
        pending.DriverInfoJson = JsonSerializer.Serialize(driverInfo);
        pending.Step = 6;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetVehicleInfoAsync(VehicleInfoDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 6 || pending.Role != "Driver") return false;

        try
        {
            // Upload vehicle photos to S3 before saving data
            var shasehPhotoUrl = await UploadPhotoToS3Async(dto.ShasehPhoto, "vehicle-shaseh");
            var efragPhotoUrl = await UploadPhotoToS3Async(dto.EfragPhoto, "vehicle-efrag");

            // Use mapper to convert DTO to VehicleInfoData
            var vehicleInfo = dto.ToVehicleInfoData();

            // Update photo URLs with S3 URLs
            vehicleInfo.ShasehPhotoUrl = shasehPhotoUrl;
            vehicleInfo.EfragPhotoUrl = efragPhotoUrl;

            pending.VehicleInfoJson = JsonSerializer.Serialize(vehicleInfo);
            pending.Step = 7;
            pending.UpdatedAt = DateTime.UtcNow;
            repo.Update(pending);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Vehicle info set successfully for phone: {PhoneNumber}", dto.PhoneNumber);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting vehicle info for phone: {PhoneNumber}", dto.PhoneNumber);
            return false;
        }
    }

    public async Task<bool> SetResidenceInfoAsync(ResidenceInfoDto dto)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pending = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == dto.PhoneNumber);
        if (pending == null || pending.Step < 7 || pending.Role != "Driver") return false;

        var residenceInfo = dto.ToResidenceInfoData();
        pending.ResidenceInfoJson = JsonSerializer.Serialize(residenceInfo);
        pending.Step = 8;
        pending.UpdatedAt = DateTime.UtcNow;
        repo.Update(pending);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<PendingRegistrationResponseDto?> GetPendingRegistrationByPhoneAsync(string phoneNumber)
    {
        var repo = _unitOfWork.Repository<PendingRegistration>();
        var pendingRegistration = await repo.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        return pendingRegistration.ToResponseDto();
    }

    public async Task<bool> CompleteRegistrationAsync(string phoneNumber)
    {
        var pendingRepo = _unitOfWork.Repository<PendingRegistration>();
        var merchantRepo = _unitOfWork.Repository<Merchant>();
        var driverRepo = _unitOfWork.Repository<Driver>();
        var vehicleRepo = _unitOfWork.Repository<Vehicle>();
        var residenceRepo = _unitOfWork.Repository<Residence>();

        var pending = await pendingRepo.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        if (pending == null || !pending.IsPhoneVerified || string.IsNullOrEmpty(pending.Role) || string.IsNullOrEmpty(pending.Password) || string.IsNullOrEmpty(pending.BasicInfoJson))
            return false;

        // Deserialize basic info
        var basicInfo = JsonSerializer.Deserialize<BasicInfoData>(pending.BasicInfoJson);
        if (basicInfo == null)
            return false;

        // Create user using UserCreationService
        var names = basicInfo.FullName.Split(' ', 2);
        var (userCreated, userMessage, user) = await _userCreationService.CreateUserAsync(
            pending.PhoneNumber,
            pending.PhoneNumber,
            pending.Password,
            names[0],
            names.Length > 1 ? names[1] : string.Empty,
            basicInfo.PersonalPhotoUrl,
            basicInfo.NationalIdFrontPhotoUrl,
            basicInfo.NationalIdBackPhotoUrl,
            basicInfo.NationalIdNumber
        );

        if (!userCreated || user == null)
        {
            _logger.LogError("Failed to create user: {Message}", userMessage);
            return false;
        }

        // Create role if it doesn't exist
        if (!await _userCreationService.RoleExistsAsync(pending.Role))
        {
            var roleCreated = await _userCreationService.CreateRoleAsync(pending.Role);
            if (!roleCreated)
            {
                _logger.LogError("Failed to create role: {Role}", pending.Role);
                return false;
            }
        }

        // Assign role
        var (roleAssigned, roleMessage) = await _userCreationService.AssignRoleAsync(user, pending.Role);
        if (!roleAssigned)
        {
            _logger.LogError("Failed to assign role: {Message}", roleMessage);
            return false;
        }

        // Use UnitOfWork transaction for domain entities
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Create role-specific entities
            if (pending.Role == "Merchant" && !string.IsNullOrEmpty(pending.MerchantInfoJson))
            {
                var merchantInfo = JsonSerializer.Deserialize<MerchantInfoData>(pending.MerchantInfoJson);
                if (merchantInfo == null)
                    return false;

                var merchant = merchantInfo.ToMerchant(user.Id);
                await merchantRepo.AddAsync(merchant);
            }
            else if (pending.Role == "Driver" && !string.IsNullOrEmpty(pending.DriverInfoJson))
            {
                var driverInfo = JsonSerializer.Deserialize<DriverInfoData>(pending.DriverInfoJson);
                if (driverInfo == null)
                    return false;

                var driver = driverInfo.ToDriver(user.Id);
                await driverRepo.AddAsync(driver);
                await _unitOfWork.SaveChangesAsync(); // Save to get driver.Id

                // Vehicle (if info exists)
                if (!string.IsNullOrEmpty(pending.VehicleInfoJson))
                {
                    var vehicleInfo = JsonSerializer.Deserialize<VehicleInfoData>(pending.VehicleInfoJson);
                    if (vehicleInfo != null)
                    {
                        var vehicle = vehicleInfo.ToVehicle(driver.Id);
                        await vehicleRepo.AddAsync(vehicle);
                    }
                }
                // Residence (if info exists)
                if (!string.IsNullOrEmpty(pending.ResidenceInfoJson))
                {
                    var residenceInfo = JsonSerializer.Deserialize<ResidenceInfoData>(pending.ResidenceInfoJson);
                    if (residenceInfo != null)
                    {
                        var residence = residenceInfo.ToResidence(driver.Id);
                        await residenceRepo.AddAsync(residence);
                    }
                }
            }

            // Remove PendingRegistration
            pendingRepo.Remove(pending);
            return true;
        });
    }

    /// <summary>
    /// Generates a 6-digit verification code
    /// </summary>
    private static string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    /// <summary>
    /// Uploads a photo to S3
    /// </summary>
    /// <param name="photoFile">IFormFile containing the photo (mandatory)</param>
    /// <param name="folderName">S3 folder name for organization</param>
    /// <returns>S3 URL of the uploaded photo</returns>
    private async Task<string> UploadPhotoToS3Async(IFormFile photoFile, string folderName)
    {
        // Photos are now mandatory, so we don't need to check for null
        if (photoFile.Length == 0)
            throw new ArgumentException("Photo file cannot be empty", nameof(photoFile));

        try
        {
            // Create upload DTO with folder
            var uploadDto = new UploadPhotoDto(
                File: photoFile,
                FolderName: folderName
            );

            // Upload to S3
            var result = await _fileStorageService.UploadPhotoAsync(uploadDto);
            return result.FileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading photo to S3 for folder: {FolderName}", folderName);
            throw;
        }
    }
}


