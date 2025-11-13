
namespace BasketballAnalytics.Application.Features.Teams.Commands;


using FluentValidation;

public class CreateTeamCommandValidator :  AbstractValidator<CreateTeamCommand> 
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);
    }
}