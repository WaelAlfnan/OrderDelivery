using OrderDelivery.Application.DTOs.Requests;
using OrderDelivery.Application.DTOs.Responses;

namespace OrderDelivery.Application.Interfaces;

public interface IFileStorageService
{
    Task<UploadPhotoResponseDto> UploadPhotoAsync(UploadPhotoDto uploadDto, CancellationToken cancellationToken = default);
    Task<bool> DeletePhotoAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> PhotoExistsAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);
}
