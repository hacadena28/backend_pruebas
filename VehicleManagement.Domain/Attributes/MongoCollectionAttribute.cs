namespace VehicleManagement.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class MongoCollectionAttribute : Attribute
{
    public MongoCollectionAttribute(string name)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Collection name can not be empty.", nameof(name))
            : name;
    }

    public string Name { get; }
}
