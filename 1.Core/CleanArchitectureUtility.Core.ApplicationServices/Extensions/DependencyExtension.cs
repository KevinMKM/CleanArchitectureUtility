using System.Reflection;
using CleanArchitectureUtility.Core.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.ApplicationServices.Events;
using CleanArchitectureUtility.Core.ApplicationServices.Queries;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Core.ApplicationServices.Extensions
{
    public static class DependencyExtension
    {
        public static IServiceCollection AddApplicationServiceDependencies(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        {
            services.AddCommands(assembliesForSearch).AddQueries(assembliesForSearch).AddEvents(assembliesForSearch);
            return services;
        }

        private static IServiceCollection AddCommands(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        {
            services.AddSingleton<ICommandDispatcher, CommandDispatcher>();
            services.Scan(s => s.FromAssemblies(assembliesForSearch)
                .AddClasses(c => c.AssignableToAny(typeof(ICommandHandler<>), typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services;
        }

        private static IServiceCollection AddQueries(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        {
            services.AddSingleton<IQueryDispatcher, QueryDispatcher>();
            services.Scan(s => s.FromAssemblies(assembliesForSearch)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddEvents(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        {
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.Scan(s => s.FromAssemblies(assembliesForSearch)
                .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}