﻿using System.Reflection;
using CleanArchitectureUtility.Core.Contract.Data.Commands;
using CleanArchitectureUtility.Core.Contract.Data.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.WebApi.Extensions.DependencyInjection;

public static class AddDataAccessExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch) 
        => services.AddRepositories(assembliesForSearch).AddUnitOfWorks(assembliesForSearch);

    public static IServiceCollection AddRepositories(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(ICommandRepository<>), typeof(IQueryRepository));

    public static IServiceCollection AddUnitOfWorks(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
        => services.AddWithTransientLifetime(assembliesForSearch, typeof(IUnitOfWork));
}