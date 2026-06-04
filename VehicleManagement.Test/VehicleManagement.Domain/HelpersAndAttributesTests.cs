using VehicleManagement.Domain.Attributes;
using VehicleManagement.Domain.Common.Helpers;
using VehicleManagement.Domain.Common.Wrappers;

namespace VehicleManagement.Test.VehicleManagement.Domain;

public sealed class HelpersAndAttributesTests
{
    [Fact]
    public void ExpressionExtensions_AndCombinesPredicates()
    {
        System.Linq.Expressions.Expression<Func<int, bool>> greaterThanTwo = value => value > 2;
        System.Linq.Expressions.Expression<Func<int, bool>> even = value => value % 2 == 0;

        var predicate = greaterThanTwo.And(even).Compile();

        Assert.False(predicate(2));
        Assert.True(predicate(4));
    }

    [Fact]
    public void ExpressionExtensions_OrCombinesPredicatesWithSingleParameter()
    {
        System.Linq.Expressions.Expression<Func<string, bool>> startsWithA = value => value.StartsWith("a");
        System.Linq.Expressions.Expression<Func<string, bool>> endsWithZ = other => other.EndsWith("z");

        var predicate = startsWithA.Or(endsWithZ).Compile();

        Assert.True(predicate("alpha"));
        Assert.True(predicate("jazz"));
        Assert.False(predicate("middle"));
    }

    [Fact]
    public void TextProcessingHelper_RemoveDiacriticsStripsAccents()
    {
        var result = TextProcessingHelper.RemoveDiacritics("áéíóú ñ Ç");

        Assert.Equal("aeiou n C", result);
    }

    [Fact]
    public void PaginatedResponse_CalculatesTotalPagesAndStoresMetadata()
    {
        var response = new PaginatedResponse<int>([1, 2, 3], pageNumber: 2, pageSize: 10, totalRecords: 21, totalCount: 50);

        Assert.True(response.Success);
        Assert.Equal(2, response.PageNumber);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(21, response.TotalRecords);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(50, response.TotalCountRecords);
        Assert.Equal([1, 2, 3], response.Data);
    }

    [Fact]
    public void PaginatedResponse_ErrorConstructorPreservesStatus()
    {
        var response = new PaginatedResponse<int>(400, "invalid", false);

        Assert.False(response.Success);
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("invalid", response.Message);
    }

    [Fact]
    public void MongoCollectionAttribute_StoresNameAndRejectsBlankValues()
    {
        var attribute = new MongoCollectionAttribute("Users");

        Assert.Equal("Users", attribute.Name);
        Assert.Throws<ArgumentException>(() => new MongoCollectionAttribute(""));
        Assert.Throws<ArgumentException>(() => new MongoCollectionAttribute(" "));
    }

    [Fact]
    public void ApiResponse_AllowsSettingTransportShape()
    {
        var response = new ApiResponse<string>
        {
            StatusCode = 200,
            Success = true,
            Data = "data",
            Message = "ok",
            Errors = ["none"]
        };

        Assert.Equal(200, response.StatusCode);
        Assert.True(response.Success);
        Assert.Equal("data", response.Data);
        Assert.Equal("ok", response.Message);
        Assert.Equal("none", Assert.Single(response.Errors));
    }
}
