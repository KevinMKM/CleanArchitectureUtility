using CleanArchitectureUtility.Utilities.Swagger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CleanArchitectureUtility.Utilities.Swagger;

public static class SwaggerServiceCollectionX
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, string sectionName)
        => services.AddSwagger(configuration.GetSection(sectionName));

    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SwaggerOption>(configuration);
        var option = configuration.Get<SwaggerOption>() ?? new SwaggerOption();
        return services.AddService(option);
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerOption> action)
    {
        services.Configure(action);
        var option = new SwaggerOption();
        action.Invoke(option);
        return services.AddService(option);
    }

    private static IServiceCollection AddService(this IServiceCollection services, SwaggerOption option)
    {
        if (option.Enabled)
        {
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc(option.Name, new OpenApiInfo
                {
                    Title = option.Title,
                    Version = option.Version,
                    Description = option.Description
                });

                if (option.EnabledSecurities.Count == 0) 
                    return;
                option.EnabledSecurities.ForEach(security =>
                {
                    setup.AddSecurityDefinition(security.Scheme, security.ToOpenApiSecurityScheme());
                    setup.AddSecurityRequirement(security.ToOpenApiSecurityRequirement());
                });

                setup.OperationFilter<SecurityRequirementsOperationFilter>(option);
            });
        }

        return services;
    }

    public static void UseSwagger(this WebApplication app)
    {
        var option = app.Services.GetRequiredService<IOptions<SwaggerOption>>().Value;
        if (!option.Enabled) 
            return;
        app.UseSwagger();
        app.UseSwaggerUI(setup =>
        {
            setup.RoutePrefix = option.RoutePrefix;
            setup.SwaggerEndpoint($"/swagger/{option.Name}/swagger.json", option.Title);
            if (option.OAuthConfig.UsePkce) 
                setup.OAuthUsePkce();
        });
    }
}