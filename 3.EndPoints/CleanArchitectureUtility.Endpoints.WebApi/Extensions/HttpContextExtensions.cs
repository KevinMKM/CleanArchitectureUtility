using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.WebApi.Extensions;

public static class HttpContextExtensions
{
    public static ICommandDispatcher CommandDispatcher(this HttpContext httpContext) 
        => httpContext.RequestServices.GetRequiredService<ICommandDispatcher>();

    public static IQueryDispatcher QueryDispatcher(this HttpContext httpContext)
        => httpContext.RequestServices.GetRequiredService<IQueryDispatcher>();

    public static IEventDispatcher EventDispatcher(this HttpContext httpContext)
        => httpContext.RequestServices.GetRequiredService<IEventDispatcher>();
}