namespace OrderDelivery.Application.DTOs.Responses;
 
public record ApiResponse<T>(T? Data, string Message, bool Success = true, IEnumerable<string>? Errors = null); 