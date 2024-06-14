using System.Reflection;
using CleanArchitectureUtility.Core.Abstractions.ObjectMappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace CleanArchitectureUtility.Utilities.ObjectMappers.AutoMapper;
public static class AutoMapperServiceCollectionX
{
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services, IConfiguration configuration, string sectionName)
        => services.AddAutoMapperProfiles(configuration.GetSection(sectionName));

    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services, IConfiguration configuration)
    {
        var option = configuration.Get<AutoMapperOption>();
        var assemblies = GetAssemblies(option.AssemblyNamesForLoadProfiles);
        return services.AddAutoMapper(assemblies).AddSingleton<IMapperAdapter, AutoMapperAdapter>();
    }

    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services, Action<AutoMapperOption> setupAction)
    {
        var option = new AutoMapperOption();
        setupAction.Invoke(option);
        var assemblies = GetAssemblies(option.AssemblyNamesForLoadProfiles);
        return services.AddAutoMapper(assemblies).AddSingleton<IMapperAdapter, AutoMapperAdapter>();
    }

    private static List<Assembly> GetAssemblies(string assemblyNames)
    {
        var assemblies = new List<Assembly>();
        var dependencies = DependencyContext.Default.RuntimeLibraries;

        foreach (var library in dependencies)
        {
            if (!IsCandidateCompilationLibrary(library, assemblyNames.Split(','))) 
                continue;
            var assembly = Assembly.Load(new AssemblyName(library.Name));
            assemblies.Add(assembly);
        }

        return assemblies;
    }

    private static bool IsCandidateCompilationLibrary(Library compilationLibrary, string[] assemblyName)
        => assemblyName.Any(compilationLibrary.Name.Contains)
           || compilationLibrary.Dependencies.Any(d => assemblyName.Any(c => d.Name.Contains(c)));
}