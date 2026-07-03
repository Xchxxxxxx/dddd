using Microsoft.Extensions.DependencyInjection;

namespace DDD.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class InjectableAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public Type? InterfaceType { get; }

    public InjectableAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public InjectableAttribute(ServiceLifetime lifetime, Type interfaceType)
    {
        Lifetime = lifetime;
        InterfaceType = interfaceType;
    }
}