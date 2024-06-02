using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Queries;
using CleanArchitectureUtility.Utilities.Utilities;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureUtility.Endpoints.API.Extensions;

public static class HttpContextExtentions
{
    public static ICommandDispatcher CommandDispatcher(this HttpContext httpContext) =>
        (ICommandDispatcher)httpContext.RequestServices.GetService(typeof(ICommandDispatcher));

    public static IQueryDispatcher QueryDispatcher(this HttpContext httpContext) =>
        (IQueryDispatcher)httpContext.RequestServices.GetService(typeof(IQueryDispatcher));

    public static IEventDispatcher EventDispatcher(this HttpContext httpContext) =>
        (IEventDispatcher)httpContext.RequestServices.GetService(typeof(IEventDispatcher));

    public static CommonService CommonService(this HttpContext httpContext) =>
        (CommonService)httpContext.RequestServices.GetService(typeof(CommonService));
}