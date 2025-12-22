using MediatR;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using BasketballAnalytics.Application.Features.Teams.Commands;
using Microsoft.EntityFrameworkCore;
using BasketballAnalytics.Application.Exceptions;


namespace BasketballAnalytics.Application.Features.Authentication.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RegisterUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    { 
         if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            throw new UserAlreadyExistsException(request.Username);
        }
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}