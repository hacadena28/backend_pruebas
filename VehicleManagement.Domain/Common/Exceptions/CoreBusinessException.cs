namespace VehicleManagement.Domain.Common.Exceptions;

public class CoreBusinessException : Exception
{
    public CoreBusinessException()
    {
    }

    public CoreBusinessException(string message)
        : base(message)
    {
    }

    public CoreBusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
