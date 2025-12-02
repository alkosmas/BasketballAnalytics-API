using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BasketballAnalytics.Application.Features.Teams.Queries
{
    public record LoginQuery
    (
        string Username,
        string Password
    ): IRequest<string>;
}


