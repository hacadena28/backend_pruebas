using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Infrastructure.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;

namespace VehicleManagement.Test.VehicleManagement.Infraestructure;

public sealed class AppExceptionFilterAttributeTests
{
    public static TheoryData<Exception, int, string> ExceptionCases => new()
    {
        { new ValidationException("validation failed"), StatusCodes.Status400BadRequest, nameof(ValidationException) },
        { new UnauthorizedException("unauthorized"), StatusCodes.Status401Unauthorized, nameof(UnauthorizedException) },
        { new CoreBusinessException("business"), StatusCodes.Status400BadRequest, nameof(CoreBusinessException) },
        { new NotFoundException("missing"), StatusCodes.Status404NotFound, nameof(NotFoundException) },
        { new ForbiddenException("forbidden"), StatusCodes.Status403Forbidden, nameof(ForbiddenException) },
        { new AlreadyExistException("exists"), StatusCodes.Status400BadRequest, nameof(AlreadyExistException) },
        { new ConflictException("conflict"), StatusCodes.Status409Conflict, nameof(ConflictException) },
        { new CustomException("custom"), StatusCodes.Status400BadRequest, nameof(CustomException) },
        { new InvalidOperationException("unexpected"), StatusCodes.Status500InternalServerError, nameof(InvalidOperationException) }
    };

    [Theory]
    [MemberData(nameof(ExceptionCases))]
    public void OnException_MapsKnownExceptionsToErrorResponse(Exception exception, int expectedStatusCode, string expectedError)
    {
        var sut = new AppExceptionFilterAttribute(NullLogger<AppExceptionFilterAttribute>.Instance);
        var context = CreateExceptionContext(exception);

        sut.OnException(context);

        Assert.Equal(expectedStatusCode, context.HttpContext.Response.StatusCode);
        var objectResult = Assert.IsType<ObjectResult>(context.Result);
        var response = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Equal(exception.Message, response.Message);
        Assert.Equal(expectedError, response.Error);
    }

    [Fact]
    public void OnException_ReturnsWithoutThrowing_WhenContextIsNull()
    {
        var sut = new AppExceptionFilterAttribute(NullLogger<AppExceptionFilterAttribute>.Instance);

        sut.OnException(null!);
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, [])
        {
            Exception = exception
        };
    }
}
