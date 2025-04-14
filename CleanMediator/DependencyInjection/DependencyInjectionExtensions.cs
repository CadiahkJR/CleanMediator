using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using CleanMediator.Abstractions;
using CleanMediator.Dispatching;

namespace CleanMediator.DependencyInjection;

/// <summary>
/// Provides extension methods for registering CleanMediator services into the .NET dependency injection container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the CleanMediator dispatcher, request handlers, and pipeline behaviors found in the calling assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    public static IServiceCollection AddCleanMediator(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        services.AddScoped<IRequestDispatcher, RequestDispatcher>();

        RegisterImplementations(services, typeof(IRequestHandler<>), assembly);
        RegisterImplementations(services, typeof(IPipelineBehavior<>), assembly);

        RegisterImplementations(services, typeof(IRequestHandler<,>), assembly);
        RegisterImplementations(services, typeof(IPipelineBehavior<,>), assembly);

        return services;
    }

    #region Register Implementations for IRequestHandler and IPipelineBehavior
    /// <summary>
    /// Registers all implementations of a given open generic interface type found in the provided assembly.
    /// This includes both open-generic and closed-generic implementations.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="openGenericType">The open generic interface type (e.g., <c>typeof(IRequestHandler&lt;,&gt;)</c>).</param>
    /// <param name="assembly">The assembly to scan for implementations.</param>
    private static void RegisterImplementations(IServiceCollection services, Type openGenericType, Assembly assembly)
    {
        var openImplTypes = assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                t.IsGenericTypeDefinition &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == openGenericType));

        foreach (var impl in openImplTypes)
        {
            services.AddScoped(openGenericType, impl);
        }

        var concreteImpls = assembly
            .GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                !t.IsGenericTypeDefinition)
            .SelectMany(t => t.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == openGenericType)
                .Select(i => new { Service = i, Implementation = t }));

        foreach (var pair in concreteImpls)
        {
            services.AddScoped(pair.Service, pair.Implementation);
        }
    }
    #endregion`
}
