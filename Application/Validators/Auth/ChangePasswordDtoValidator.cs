using Application.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Auth;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Current password is required");
            
        RuleFor(x => x.NewPassword)
            .Matches(ValidationConstants.Password.Pattern)
            .WithMessage(ValidationConstants.Password.ErrorMessage);
    }
}
