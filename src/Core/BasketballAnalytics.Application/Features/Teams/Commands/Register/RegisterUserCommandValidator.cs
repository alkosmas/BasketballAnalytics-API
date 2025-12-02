using BasketballAnalytics.Application.Features.Teams.Commands;
using FluentValidation;

namespace BasketballAnalytics.Application.Features.Authentication.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty().Must(role => role == "Admin" || role == "User")
            .WithMessage("Role must be either 'Admin' or 'User'.");
    }
}