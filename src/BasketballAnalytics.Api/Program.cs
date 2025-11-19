using BasketballAnalytics.Api.Middleware;
using BasketballAnalytics.Application.Common.Behavior;
using BasketballAnalytics.Application.Common.Behaviors;
using BasketballAnalytics.Application.Common.Interfaces;
using BasketballAnalytics.Application.Features.Teams.Queries;
using BasketballAnalytics.Persistence.Authentication;
using BasketballAnalytics.Persistence.DbContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddMediatR(cfg =>
{
     cfg.RegisterServicesFromAssembly(typeof(GetAllTeamsQuery).Assembly);
     cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddValidatorsFromAssembly(typeof(GetAllTeamsQuery).Assembly);
builder.Services.AddMemoryCache();

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);

builder.Services.AddSingleton(Options.Create(jwtSettings));

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    });


var app = builder.Build();
app.UseMiddleware<GlobalErrorHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
