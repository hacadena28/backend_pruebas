namespace VehicleManagement.Infrastructure.Extensions;

public static class ApiConstants
{
    public const string LocalEnviroment = "Local";
    public const string ApplicationProject = "VehicleManagement.Application";
    public const string DomainProject = "VehicleManagement.Domain";
    public const string Infrastructure = "VehicleManagement.Infrastructure";
    public const string MalformedToken = "El token de autenticación tiene un formato no válido.";
    public const string InvalidSignature = "La firma del token de autenticación no es válida.";
    public const string ExpiredToken = "El token de autenticación ha expirado.";
    public const string UnauthorizedToken = "El token de autenticación no es válido.";
    public const string NoPermissions = "El usuario no tiene permisos para acceder a este recurso.";
}