namespace BasketballAnalytics.Application.Features.Teams.Commands;

using FluentValidation;

public class DeleteTeamCommandValidation : AbstractValidator<DeleteTeamCommand>
{
    public DeleteTeamCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}