using MongoDB.Bson.Serialization.Attributes;
using VehicleManagement.Domain.Entities.Base;

namespace VehicleManagement.Domain.Entities;

public class Vehiculo : BaseEntity<string>
{
    [BsonElement("Placa")]
    public string Placa { get; set; } = string.Empty;

    [BsonElement("Marca")]
    public string Marca { get; set; } = string.Empty;

    [BsonElement("Modelo")]
    public string Modelo { get; set; } = string.Empty;

    [BsonElement("Anio")]
    public int Anio { get; set; }

    [BsonElement("Color")]
    public string Color { get; set; } = string.Empty;

    [BsonElement("FechaRegistro")]
    public DateTime FechaRegistro { get; set; }
}
