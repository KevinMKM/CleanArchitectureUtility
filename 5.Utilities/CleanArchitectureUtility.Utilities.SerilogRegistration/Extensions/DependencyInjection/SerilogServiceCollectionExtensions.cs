using CleanArchitectureUtility.Utilities.SerilogRegistration.Enrichers;
using CleanArchitectureUtility.Utilities.SerilogRegistration.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using ActivityEnricher = CleanArchitectureUtility.Utilities.SerilogRegistration.Enrichers.ActivityEnricher;

namespace CleanArchitectureUtility.Utilities.SerilogRegistration.Extensions.DependencyInjection;

public static class SerilogServiceCollectionExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, params Type[] enrichersType)
    {
        builder.Services.Configure<SerilogApplicationEnricherOptions>(configuration);
        return AddServices(builder, enrichersType);
    }

    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string sectionName, params Type[] enrichersType)
        => builder.AddSerilog(configuration.GetSection(sectionName), enrichersType);

    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, Action<SerilogApplicationEnricherOptions> setupAction, params Type[] enrichersType)
    {
        builder.Services.Configure(setupAction);
        return AddServices(builder, enrichersType);
    }

    private static WebApplicationBuilder AddServices(WebApplicationBuilder builder, params Type[] enrichersType)
    {
        List<ILogEventEnricher> logEventEnrichers = new();
        builder.Services.AddTransient<UserInfoEnricher>();
        builder.Services.AddTransient<ApplicaitonEnricher>();
        builder.Services.AddTransient<ActivityEnricher>();
        foreach (var enricherType in enrichersType)
            builder.Services.AddTransient(enricherType);

        builder.Host.UseSerilog((ctx, services, lc) =>
        {
            logEventEnrichers.Add(services.GetRequiredService<UserInfoEnricher>());
            logEventEnrichers.Add(services.GetRequiredService<ApplicaitonEnricher>());
            logEventEnrichers.Add(services.GetRequiredService<ActivityEnricher>());
            logEventEnrichers.AddRange(enrichersType.Select(enricherType => services.GetRequiredService(enricherType) as ILogEventEnricher));
            lc.Enrich.FromLogContext()
                .Enrich.With(logEventEnrichers.ToArray())
                .Enrich.WithExceptionDetails()
                .Enrich.WithSpan()
                .ReadFrom.Configuration(ctx.Configuration);
        });
        return builder;
    }
}