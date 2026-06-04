namespace VehicleManagement.Domain.Common.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException(string message)
       : base(message, null)
    {
    }
}