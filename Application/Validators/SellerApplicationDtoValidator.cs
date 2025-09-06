using Application.DTOs.store;
using FluentValidation;

namespace Application.Validators;

public class SellerApplicationDtoValidator: AbstractValidator<SellerApplicationDto>
{
    public SellerApplicationDtoValidator()
    {
        RuleFor(s => s.Address).MaximumLength(250)
            .NotEmpty();
        RuleFor(s => s.PhoneNumber).MaximumLength(15);
        RuleFor(s => s.DateOfBirth).NotEmpty();
        RuleFor(s => s.Note).MaximumLength(500).NotEmpty();
        RuleFor(s => s.IdCardUrl)
            .NotEmpty()
            .Matches(@"^(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$");
        RuleFor(s => s.PostalCode).MaximumLength(10);
    }
}