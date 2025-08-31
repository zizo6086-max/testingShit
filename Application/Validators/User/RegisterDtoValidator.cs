using Application.DTOs;
using FluentValidation;

namespace Application.Validators.User;

public class RegisterDtoValidator:AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(m => m.Username).Matches("^[a-zA-Z][a-zA-Z0-9_]{2,19}$");
        RuleFor(m => m.Email).EmailAddress();
        RuleFor(m => m.Password).Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$");
    }
}