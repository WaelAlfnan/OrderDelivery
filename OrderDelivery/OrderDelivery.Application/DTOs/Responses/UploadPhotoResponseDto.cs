namespace OrderDelivery.Application.DTOs.Responses;

public record UploadPhotoResponseDto(
    string FileUrl,
    string FileName,
    long FileSize,
    string ContentType
);
