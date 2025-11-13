using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketballAnalytics.Application.Features.Teams.Commands;

using FluentValidation;


public class UpdateTeamCommandValidation : AbstractValidator<UpdateTeamCommand>
{
        public UpdateTeamCommandValidation()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        }
}
    
