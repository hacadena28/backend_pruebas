namespace VehicleManagement.Application.Vehiculos.Dtos;

public record VehiculoDto(
    string Id,
    string Placa,
    string Marca,
    string Modelo,
    int Anio,
    string Color,
    DateTime FechaRegistro
);
