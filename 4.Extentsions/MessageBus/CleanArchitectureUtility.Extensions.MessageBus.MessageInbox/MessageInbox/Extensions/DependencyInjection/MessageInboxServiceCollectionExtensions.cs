﻿using CleanArchitectureUtility.Extensions.Abstractions.MessageBus;
using CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.MessageInbox.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Extensions.MessageBus.MessageInbox.MessageInbox.Extensions.DependencyInjection;

public static class MessageInboxServiceCollectionExtensions
{
    public static IServiceCollection AddMessageInbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageInboxOptions>(configuration);
        AddServices(services);
        return services;
    }

    public static IServiceCollection AddMessageInbox(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddMessageInbox(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddMessageInbox(this IServiceCollection services, Action<MessageInboxOptions> setupAction)
    {
        services.Configure(setupAction);
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IMessageConsumer, InboxMessageConsumer>();
    }
}