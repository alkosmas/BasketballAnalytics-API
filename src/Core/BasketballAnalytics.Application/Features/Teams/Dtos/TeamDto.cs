using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketballAnalytics.Application.Features.Teams.Dtos
{
    public class TeamDto
    {
        public Guid Id { get; set;}
        public string Name { get; set;}

        public string City{ get; set;}
    }
}