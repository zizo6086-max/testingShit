using Application.DTOs;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(m => m.Username)
            .Matches(ValidationConstants.Username.Pattern)
            .WithMessage(ValidationConstants.Username.ErrorMessage);
            
        RuleFor(m => m.Email)
            .EmailAddress()
            .WithMessage(ValidationConstants.Email.ErrorMessage);
            
        RuleFor(m => m.Password)
            .Matches(ValidationConstants.Password.Pattern)
            .WithMessage(ValidationConstants.Password.ErrorMessage);
    }
}