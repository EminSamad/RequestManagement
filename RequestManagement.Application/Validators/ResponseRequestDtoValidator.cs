using FluentValidation;
using RequestManagement.Core.DTOs.Request;

namespace RequestManagement.Application.Validators;

public class ResponseRequestDtoValidator : AbstractValidator<ResponseRequestDto>
{
    public ResponseRequestDtoValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("RequestId must be greater than 0");

        RuleFor(x => x.ResponseText)
            .NotEmpty().WithMessage("Response text is required")
            .MaximumLength(1000).WithMessage("Response text cannot exceed 1000 characters");
    }
}