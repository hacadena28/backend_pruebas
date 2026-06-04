using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VehicleManagement.Domain.Entities.Base;

public class BaseEntity<T> : DomainEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public virtual T Id { get; set; } = default!;
}
