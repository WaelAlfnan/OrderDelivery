using Microsoft.AspNetCore.Http;
using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;

namespace OrderDelivery.Application.Mappers.RegistrationStepsMappers;

public static class UploadPhotoMapper
{
    public static UploadPhotoResponseDto ToUploadPhotoResponseDto(
        IFormFile file, 
        string fileUrl)
    {
        return new UploadPhotoResponseDto(
            FileUrl: fileUrl,
            FileName: file.FileName,
            FileSize: file.Length,
            ContentType: file.ContentType
        );
    }
}
