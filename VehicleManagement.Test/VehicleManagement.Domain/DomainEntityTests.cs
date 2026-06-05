using VehicleManagement.Domain.Entities.Base;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Domain;

public class DomainEntityTests
{
    [Fact]
    public void MarkAsCreated_SetsCreatedAtToNow()
    {
        // Arrange
        var entity = new DomainEntity();
        var before = DateTime.UtcNow;

        // Act
        entity.MarkAsCreated();
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(entity.CreatedAt >= before && entity.CreatedAt <= after);
    }

    [Fact]
    public void MarkAsUpdated_SetsUpdatedAtToNow()
    {
        // Arrange
        var entity = new DomainEntity();
        var before = DateTime.UtcNow;

        // Act
        entity.MarkAsUpdated();
        var after = DateTime.UtcNow;

        // Assert
        Assert.True(entity.UpdatedAt >= before && entity.UpdatedAt <= after);
    }

    [Fact]
    public void MarkAsDeleted_SetsIsDeletedToTrue()
    {
        // Arrange
        var entity = new DomainEntity();

        // Act
        entity.MarkAsDeleted();

        // Assert
        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public void SetDelete_SetsDeletedOnToNow()
    {
        // Arrange
        var entity = new DomainEntity();
        var before = DateTime.UtcNow;

        // Act
        entity.SetDelete();
        var after = DateTime.UtcNow;

        // Assert
        Assert.NotNull(entity.DeletedOn);
        Assert.True(entity.DeletedOn >= before && entity.DeletedOn <= after);
    }
}
