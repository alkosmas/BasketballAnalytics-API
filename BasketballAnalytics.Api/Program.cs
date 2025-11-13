using BasketballAnalytics.Api.Middleware;
using BasketballAnalytics.Application.Common.Behavior;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Teams.Queries;
using BasketballAnalytics.Persistence.DbContext;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("BasketballDb"));
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddMediatR(cfg =>
{
     cfg.RegisterServicesFromAssembly(typeof(GetAllTeamsQuery).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssembly(typeof(GetAllTeamsQuery).Assembly);


var app = builder.Build();
app.UseMiddleware<GlobalErrorHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
      using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated(); //it will create base if it hasn't have base 
        if (!context.Teams.Any())
        {
            context.Teams.Add(new BasketballAnalytics.Domain.Entities.Team { Id = Guid.NewGuid(), Name = "Olympiacos", City = "Piraeus" });
            context.Teams.Add(new BasketballAnalytics.Domain.Entities.Team { Id = Guid.NewGuid(), Name = "Panathinaikos", City = "Athens" });
            context.SaveChanges();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
