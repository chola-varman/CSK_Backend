using CskMasala.Identity.Application.Commands;
using FluentValidation;

namespace CskMasala.Identity.Application.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirebaseIdToken).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
    }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).MaximumLength(15).When(x => x.PhoneNumber != null);
    }
}
