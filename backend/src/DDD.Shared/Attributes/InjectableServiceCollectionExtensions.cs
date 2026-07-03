using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DDD.Shared.Attributes;

public static class InjectableServiceCollectionExtensions
{
    public static IServiceCollection AddInjectableServices(
        this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .ToList();

        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<InjectableAttribute>(false).ToList();

            foreach (var attr in attributes)
            {
                if (attr.InterfaceType is not null)
                {
                    if (!attr.InterfaceType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException(
                            $"类型 {type.FullName} 未实现接口 {attr.InterfaceType.FullName}");
                    }

                    Register(services, attr.InterfaceType, type, attr.Lifetime);
                }
                else
                {
                    Register(services, type, type, attr.Lifetime);
                }
            }

            if (attributes.Count == 0)
            {
                var interfaces = type.GetInterfaces()
                    .Where(i => i != typeof(IDisposable))
                    .ToList();

                foreach (var iface in interfaces)
                {
                    var interfaceAttr = iface.GetCustomAttribute<InjectableAttribute>(false);
                    if (interfaceAttr is not null)
                    {
                        Register(services, iface, type, interfaceAttr.Lifetime);
                    }
                }
            }
        }

        return services;
    }

    private static void Register(
        IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime)
    {
        switch (lifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(serviceType, implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(serviceType, implementationType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(serviceType, implementationType);
                break;
        }
    }
}