using MongoDB.Bson.Serialization.Attributes;

namespace VehicleManagement.Domain.Entities.Base;

public class DomainEntity : ISoftDelete
{
    [BsonElement("CreadoEl")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("ActualizadoEl")]
    public DateTime UpdatedAt { get; private set; }

    [BsonElement("EliminadoEl")]
    [BsonIgnoreIfNull]
    public DateTime? DeletedOn { get; set; }

    [BsonElement("EstaEliminado")]
    public bool IsDeleted { get; private set; }

    public void MarkAsCreated() => CreatedAt = DateTime.UtcNow;

    public void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    public void MarkAsDeleted() => IsDeleted = true;

    public void SetDelete() => DeletedOn = DateTime.UtcNow;
}
