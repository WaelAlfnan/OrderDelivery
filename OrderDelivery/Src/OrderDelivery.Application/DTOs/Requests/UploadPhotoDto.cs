using Microsoft.AspNetCore.Http;

namespace OrderDelivery.Application.DTOs.Requests;

public record UploadPhotoDto(
    IFormFile File,
    string? FolderName = null
);
