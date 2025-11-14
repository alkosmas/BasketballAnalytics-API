
namespace BasketballAnalytics.Application.Features.Teams.Commands;

using BasketballAnalytics.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class CreateTeamCommandValidator :  AbstractValidator<CreateTeamCommand> 
{
        private readonly IApplicationDbContext _context; 

    public CreateTeamCommandValidator(IApplicationDbContext context)
    {
         _context = context;

        RuleFor(x => x.Name).NotEmpty().WithMessage("Team name is required")
        .MaximumLength(50).WithMessage("Team name must have 50 characters");

        RuleFor(x => x.Name).MustAsync(BeUniqueName)
        .WithMessage("A team with this name already exists");

        RuleFor(x => x.City).NotEmpty().WithMessage("Team City is required")
        .MaximumLength(50).WithMessage("Team name must have 50 characters");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Teams.AnyAsync(x => x.Name == name, cancellationToken);
    }
}