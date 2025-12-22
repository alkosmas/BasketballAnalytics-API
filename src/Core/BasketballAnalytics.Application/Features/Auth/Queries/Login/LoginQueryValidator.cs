using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace BasketballAnalytics.Application.Features.Teams.Queries
{
    public class LoginQueryValidator : AbstractValidator<LoginQuery>

    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}