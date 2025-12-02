using BasketballAnalytics.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTeamCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);

        RuleFor(x => x) 
            .MustAsync(BeUniqueNameForUpdate)
            .WithMessage("A team with this name already exists.")
            .WithName("Name"); 
    }
    private async Task<bool> BeUniqueNameForUpdate(UpdateTeamCommand command, CancellationToken cancellationToken)
    {
        return !await _context.Teams
            .AnyAsync(t => t.Name == command.Name && t.Id != command.Id, cancellationToken);
    }
}