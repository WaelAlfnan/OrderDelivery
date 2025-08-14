using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderDelivery.Application.Configurations;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;
using OrderDelivery.Application.Interfaces;
using OrderDelivery.Application.Mappers.RegistrationStepsMappers;

namespace OrderDelivery.Application.Services;

public class S3FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3Settings _s3Settings;
    private readonly ILogger<S3FileStorageService> _logger;

    public S3FileStorageService(
        IAmazonS3 s3Client,
        IOptions<S3Settings> s3Settings,
        ILogger<S3FileStorageService> logger)
    {
        _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
        _s3Settings = s3Settings?.Value ?? throw new ArgumentNullException(nameof(s3Settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Validate S3 settings
        if (string.IsNullOrEmpty(_s3Settings.BucketName))
        {
            throw new InvalidOperationException("S3 bucket name is not configured");
        }

        if (string.IsNullOrEmpty(_s3Settings.Region))
        {
            throw new InvalidOperationException("S3 region is not configured");
        }

        _logger.LogInformation("S3FileStorageService initialized for bucket: {BucketName} in region: {Region}", 
            _s3Settings.BucketName, _s3Settings.Region);
    }

    public async Task<UploadPhotoResponseDto> UploadPhotoAsync(
        UploadPhotoDto uploadDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Generate unique file name
            var fileExtension = Path.GetExtension(uploadDto.File.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            // Create folder path if specified
            var key = string.IsNullOrEmpty(uploadDto.FolderName)
                ? fileName
                : $"{uploadDto.FolderName.Trim('/')}/{fileName}";

            // Upload file to S3
            using var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(
                uploadDto.File.OpenReadStream(),
                _s3Settings.BucketName,
                key,
                cancellationToken);

            // Generate file URL
            var fileUrl = $"{_s3Settings.BaseUrl.TrimEnd('/')}/{key}";

            _logger.LogInformation("File uploaded successfully to S3. Key: {Key}, URL: {Url}", key, fileUrl);

            return UploadPhotoMapper.ToUploadPhotoResponseDto(uploadDto.File, fileUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to S3. FileName: {FileName}", uploadDto.File.FileName);
            throw new InvalidOperationException("Failed to upload file to S3", ex);
        }
    }

    public async Task<bool> DeletePhotoAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // Extract key from URL
            var key = ExtractKeyFromUrl(fileUrl);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);

            _logger.LogInformation("File deleted successfully from S3. Key: {Key}", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from S3. URL: {Url}", fileUrl);
            return false;
        }
    }

    public async Task<bool> PhotoExistsAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = ExtractKeyFromUrl(fileUrl);

            var request = new GetObjectMetadataRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if file exists in S3. URL: {Url}", fileUrl);
            return false;
        }
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ListBucketsRequest();
            await _s3Client.ListBucketsAsync(request, cancellationToken);
            _logger.LogInformation("S3 connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "S3 connection test failed");
            return false;
        }
    }

    private string ExtractKeyFromUrl(string fileUrl)
    {
        // Remove base URL to get the key
        var baseUrl = _s3Settings.BaseUrl.TrimEnd('/');
        if (fileUrl.StartsWith(baseUrl))
        {
            return fileUrl.Substring(baseUrl.Length + 1); // +1 for the '/'
        }

        // If it's already a key, return as is
        return fileUrl;
    }
}
