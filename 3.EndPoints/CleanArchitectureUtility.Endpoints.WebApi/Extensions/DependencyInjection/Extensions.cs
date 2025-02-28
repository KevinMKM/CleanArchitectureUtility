using System.Reflection;
using CleanArchitectureUtility.Core.ApplicationServices.Extensions;
using CleanArchitectureUtility.Extensions.Abstractions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace CleanArchitectureUtility.Endpoints.WebApi.Extensions.DependencyInjection;

public static class Extensions
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, params string[] assemblyNamesForSearch)
    {
        var assemblyNames = assemblyNamesForSearch.Append("CleanArchitectureUtility").ToArray();
        var assemblies = GetAssemblies(assemblyNames);
        services.AddApplicationServiceDependencies(assemblies)
            .AddDataAccess(assemblies)
            .AddUtilityServices()
            .AddCustomDependencies(assemblies);
        return services;
    }

    public static IServiceCollection AddCustomDependencies(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        => services.AddWithTransientLifetime(assemblies, typeof(ITransientLifetime))
            .AddWithScopedLifetime(assemblies, typeof(IScopeLifetime))
            .AddWithSingletonLifetime(assemblies, typeof(ISingletonLifetime));

    public static IServiceCollection AddWithTransientLifetime(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch, params Type[] assignableTo)
    {
        services.Scan(s => s.FromAssemblies(assembliesForSearch)
            .AddClasses(c => c.AssignableToAny(assignableTo))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        return services;
    }

    public static IServiceCollection AddWithScopedLifetime(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch, params Type[] assignableTo)
    {
        services.Scan(s => s.FromAssemblies(assembliesForSearch)
            .AddClasses(c => c.AssignableToAny(assignableTo))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }

    public static IServiceCollection AddWithSingletonLifetime(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch, params Type[] assignableTo)
    {
        services.Scan(s => s.FromAssemblies(assembliesForSearch)
            .AddClasses(c => c.AssignableToAny(assignableTo))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        return services;
    }

    private static List<Assembly> GetAssemblies(string[] assemblyName)
    {
        var assemblies = new List<Assembly>();
        var dependencies = DependencyContext.Default.RuntimeLibraries;
        foreach (var library in dependencies)
            if (IsCandidateCompilationLibrary(library, assemblyName))
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                assemblies.Add(assembly);
            }

        return assemblies;
    }

    private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary, string[] assemblyName) 
        => assemblyName.Any(compilationLibrary.Name.Contains)
           || compilationLibrary.Dependencies.Any(d => assemblyName.Any(c => d.Name.Contains(c)));
}