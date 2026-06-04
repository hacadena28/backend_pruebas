using FluentValidation;
using VehicleManagement.Infrastructure.Adapters;
using MediatR;

namespace VehicleManagement.Test.VehicleManagement.Infraestructure;

public sealed class ValidationBehaviourTests
{
    [Fact]
    public async Task Handle_CallsNext_WhenThereAreNoValidators()
    {
        var sut = new ValidationBehaviour<TestRequest, string>([]);
        var called = false;

        var result = await sut.Handle(new TestRequest(""), _ =>
        {
            called = true;
            return Task.FromResult("ok");
        }, CancellationToken.None);

        Assert.True(called);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_CallsNext_WhenValidatorsPass()
    {
        var sut = new ValidationBehaviour<TestRequest, string>([new TestRequestValidator()]);

        var result = await sut.Handle(new TestRequest("value"), _ => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_ThrowsValidationException_WithFailureMessages_WhenValidatorsFail()
    {
        var sut = new ValidationBehaviour<TestRequest, string>([new TestRequestValidator()]);

        var exception = await Assert.ThrowsAsync<global::VehicleManagement.Domain.Common.Exceptions.ValidationException>(() =>
            sut.Handle(new TestRequest(""), _ => Task.FromResult("ok"), CancellationToken.None));

        Assert.Contains("Name is required.", exception.Message);
        Assert.DoesNotContain("Name must be longer than two characters.", exception.Message);
    }

    public sealed record TestRequest(string Name) : IRequest<string>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(3)
                .WithMessage("Name must be longer than two characters.");
        }
    }
}
