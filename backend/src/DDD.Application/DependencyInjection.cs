using System.Reflection;
using DDD.Application.Commands.Login;
using DDD.Shared.Attributes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();

        services.AddInjectableServices(Assembly.GetExecutingAssembly());

        return services;
    }
}