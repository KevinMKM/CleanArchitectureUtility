using CleanArchitectureUtility.Extensions.Abstractions.ChangeDataLog;
using CleanArchitectureUtility.Extensions.Databases.Dapper.Options;
using CleanArchitectureUtility.Extensions.Databases.Dapper.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Extensions.Databases.Dapper.Extensions.DependencyInjection;

public static class ChangeDataLogServiceCollectionExtensions
{
    public static IServiceCollection AddChangeDataLogDalSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEntityChangeInterceptorItemRepository, DapperEntityChangeInterceptorItemRepository>();
        services.Configure<ChangeDataLogSqlOptions>(configuration);
        return services;
    }

    public static IServiceCollection AddChangeDataLogDalSql(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddChangeDataLogDalSql(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddChangeDataLogDalSql(this IServiceCollection services, Action<ChangeDataLogSqlOptions> setupAction)
    {
        services.AddScoped<IEntityChangeInterceptorItemRepository, DapperEntityChangeInterceptorItemRepository>();
        services.Configure(setupAction);
        return services;
    }
}