namespace VehicleManagement.Infrastructure.Filters;

/// <summary>
/// Response to validation of request
/// </summary>
/// <param name="StatusCode"></param>
/// <param name="Message"></param>
/// <param name="Error"></param>

public record ErrorResponse(int StatusCode, string Message, string Error);
