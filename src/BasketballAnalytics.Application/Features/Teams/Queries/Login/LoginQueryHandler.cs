using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Common.Behaviors;
using BasketballAnalytics.Application.Common.Exceptions;

namespace BasketballAnalytics.Application.Features.Teams.Queries
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery,string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginQueryHandler(IApplicationDbContext context, IJwtTokenGenerator jwtToken)
        {
            _context = context;
            _jwtTokenGenerator = jwtToken;
        }    

        public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username , cancellationToken);
            if (user is null)
            {
                throw new BadRequestException("Invalid username or password.");
            }
            
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new BadRequestException("Invalid username or password.");            
            }
            var token = _jwtTokenGenerator.GenerateToken(user);

            return token;            
        }
        
    }
}