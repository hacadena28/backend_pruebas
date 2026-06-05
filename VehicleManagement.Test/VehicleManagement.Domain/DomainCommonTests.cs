using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Domain;

public class DomainCommonTests
{
    // --- Exceptions Tests ---

    [Fact]
    public void AlreadyExistException_SetsMessage()
    {
        var msg = "Already exists";
        var ex = new AlreadyExistException(msg);
        Assert.Equal(msg, ex.Message);
        Assert.Null(ex.ErrorMessages);
    }

    [Fact]
    public void ConflictException_SetsMessage()
    {
        var msg = "Conflict";
        var ex = new ConflictException(msg);
        Assert.Equal(msg, ex.Message);
        Assert.Null(ex.ErrorMessages);
    }

    [Fact]
    public void CoreBusinessException_Constructors()
    {
        var ex1 = new CoreBusinessException();
        Assert.NotNull(ex1);

        var msg = "Business error";
        var ex2 = new CoreBusinessException(msg);
        Assert.Equal(msg, ex2.Message);

        var inner = new Exception("inner");
        var ex3 = new CoreBusinessException(msg, inner);
        Assert.Equal(msg, ex3.Message);
        Assert.Equal(inner, ex3.InnerException);
    }

    [Fact]
    public void ForbiddenException_SetsMessage()
    {
        var msg = "Forbidden";
        var ex = new ForbiddenException(msg);
        Assert.Equal(msg, ex.Message);
    }

    [Fact]
    public void InternalServerException_SetsMessageAndErrors()
    {
        var msg = "Internal error";
        var errors = new List<string> { "Error 1" };
        var ex = new InternalServerException(msg, errors);
        Assert.Equal(msg, ex.Message);
        Assert.Equal(errors, ex.ErrorMessages);
    }

    [Fact]
    public void NotFoundException_SetsMessage()
    {
        var msg = "Not found";
        var ex = new NotFoundException(msg);
        Assert.Equal(msg, ex.Message);
    }

    [Fact]
    public void UnauthorizedException_SetsMessage()
    {
        var msg = "Unauthorized";
        var ex = new UnauthorizedException(msg);
        Assert.Equal(msg, ex.Message);
    }

    [Fact]
    public void ValidationException_Constructors()
    {
        var msg = "Validation failed";
        var ex1 = new ValidationException(msg);
        Assert.Equal(msg, ex1.Message);
        Assert.Empty(ex1.Errors);

        var errors = new List<string> { "Error 1" }.ToLookup(x => "Field");
        var ex2 = new ValidationException(errors);
        Assert.Equal("Validación no superada", ex2.Message);
        Assert.Equal(errors, ex2.Errors);

        var ex3 = new ValidationException(msg, errors);
        Assert.Equal(msg, ex3.Message);
        Assert.Equal(errors, ex3.Errors);
    }

    // --- Wrappers Tests (Response<T>) ---

    [Fact]
    public void Response_DefaultConstructor()
    {
        var response = new Response<string>();
        Assert.Null(response.Message);
        Assert.False(response.Success);
    }

    [Fact]
    public void Response_StatusMessageSuccessConstructor()
    {
        var response = new Response<string>(200, "OK", true);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("OK", response.Message);
        Assert.True(response.Success);
    }

    [Fact]
    public void Response_DataMessageConstructor()
    {
        var response = new Response<string>("my data", "Success");
        Assert.Equal("my data", response.Data);
        Assert.Equal("Success", response.Message);
        Assert.True(response.Success);
    }

    [Fact]
    public void Response_StatusMessageConstructor()
    {
        var response = new Response<string>(400, "Bad Request");
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Bad Request", response.Message);
        Assert.False(response.Success);
    }

    [Fact]
    public void Response_FullConstructor()
    {
        var response = new Response<string>(201, "Created", true, "new item");
        Assert.Equal(201, response.StatusCode);
        Assert.Equal("Created", response.Message);
        Assert.True(response.Success);
        Assert.Equal("new item", response.Data);
    }

    [Fact]
    public void Response_StatusDataConstructor()
    {
        var response = new Response<int>(200, 123);
        Assert.Equal(200, response.StatusCode);
        Assert.True(response.Success);
        Assert.Equal(123, response.Data);

        var responseNull = new Response<object>(200, (object)null!);
        Assert.Null(responseNull.Data);
    }
}
