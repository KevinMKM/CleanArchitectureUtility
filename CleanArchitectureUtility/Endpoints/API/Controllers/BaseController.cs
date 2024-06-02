using System.Net;
using CleanArchitectureUtility.Core.Abstractions.Serializers;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Common;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Contracts.ApplicationServices.Queries;
using CleanArchitectureUtility.Endpoints.API.Extensions;
using CleanArchitectureUtility.Utilities.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.API.Controllers;

public class BaseController : Controller
{
    protected ICommandDispatcher CommandDispatcher => HttpContext.CommandDispatcher();
    protected IQueryDispatcher QueryDispatcher => HttpContext.QueryDispatcher();
    protected IEventDispatcher EventDispatcher => HttpContext.EventDispatcher();
    protected CommonService ApplicationContext => HttpContext.CommonService();

    public IActionResult Excel<T>(List<T> list)
    {
        var serializer = (IExcelSerializer)HttpContext.RequestServices.GetRequiredService(typeof(IExcelSerializer));
        var bytes = serializer.ListToExcelByteArray(list);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public IActionResult Excel<T>(List<T> list, string fileName)
    {
        var serializer = (IExcelSerializer)HttpContext.RequestServices.GetRequiredService(typeof(IExcelSerializer));
        var bytes = serializer.ListToExcelByteArray(list);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
    }

    protected async Task<IActionResult> Create<TCommand, TCommandResult>(TCommand command) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command);
        return result.Status == ApplicationServiceStatus.Ok
            ? StatusCode((int)HttpStatusCode.Created, result.Data)
            : BadRequest(result.Messages);
    }

    protected async Task<IActionResult> Create<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command);
        return result.Status == ApplicationServiceStatus.Ok
            ? StatusCode((int)HttpStatusCode.Created)
            : BadRequest(result.Messages);
    }

    protected async Task<IActionResult> Edit<TCommand, TCommandResult>(TCommand command) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.OK, result.Data),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Edit<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.OK),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }


    protected async Task<IActionResult> Delete<TCommand, TCommandResult>(TCommand command) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.NoContent, result.Data),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Delete<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.NoContent),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Query<TQuery, TQueryResult>(TQuery query) where TQuery : class, IQuery<TQueryResult>
    {
        var result = await QueryDispatcher.Execute<TQuery, TQueryResult>(query);
        if (result.Data == null)
            return StatusCode((int)HttpStatusCode.NoContent);
        return result.Status switch
        {
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NoContent),
            ApplicationServiceStatus.Ok => Ok(result.Data),
            _ => BadRequest(result.Messages)
        };
    }
}