namespace VehicleManagement.Domain.Common.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, null)
    {
    }
}