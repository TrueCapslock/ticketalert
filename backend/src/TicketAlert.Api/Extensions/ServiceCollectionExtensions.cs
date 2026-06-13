using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketAlert.Application.Common.Interfaces;
using TicketAlert.Application.Features.Auth;
using TicketAlert.Application.Features.Events;
using TicketAlert.Application.Features.Notifications;
using TicketAlert.Application.Features.Payments;
using TicketAlert.Application.Features.Watches;
using TicketAlert.Infrastructure.BackgroundJobs;
using TicketAlert.Infrastructure.Data;
using TicketAlert.Infrastructure.Services.Auth;
using TicketAlert.Infrastructure.Services.Notifications;
using TicketAlert.Infrastructure.Services.Payment;
using TicketAlert.Infrastructure.Services.Polling;

namespace TicketAlert.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddHttpClient<ITicketmasterService, TicketmasterService>(client =>
        {
            client.BaseAddress = new Uri("https://app.ticketmaster.com/");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddScoped<IAuthService, JwtAuthService>();
        services.AddScoped<INotificationService, EmailNotificationService>();
        services.AddScoped<IPaymentService, StripePaymentService>();

        services.AddHostedService<TicketPollingService>();
        services.AddHostedService<WatchExpirationService>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
        services.AddScoped<ILoginUserHandler, LoginUserHandler>();
        services.AddScoped<ISearchEventsHandler, SearchEventsHandler>();
        services.AddScoped<IGetEventHandler, GetEventHandler>();
        services.AddScoped<ICreateWatchHandler, CreateWatchHandler>();
        services.AddScoped<IGetUserWatchesHandler, GetUserWatchesHandler>();
        services.AddScoped<ICancelWatchHandler, CancelWatchHandler>();
        services.AddScoped<ICreateCheckoutSessionHandler, CreateCheckoutSessionHandler>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
            };
        });

        return services;
    }
}
