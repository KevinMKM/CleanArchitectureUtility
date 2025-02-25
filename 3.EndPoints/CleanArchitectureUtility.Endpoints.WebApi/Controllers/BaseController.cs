﻿using System.Net;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Commands;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Common;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Events;
using CleanArchitectureUtility.Core.Contract.ApplicationServices.Queries;
using CleanArchitectureUtility.Endpoints.WebApi.Extensions;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.WebApi.Controllers;

public class BaseController : Controller
{
    private const string ExcelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    protected ICommandDispatcher CommandDispatcher => HttpContext.CommandDispatcher();
    protected IQueryDispatcher QueryDispatcher => HttpContext.QueryDispatcher();
    protected IEventDispatcher EventDispatcher => HttpContext.EventDispatcher();

    public IActionResult Excel<T>(List<T> list)
    {
        var serializer = (IExcelSerializer)HttpContext.RequestServices.GetRequiredService(typeof(IExcelSerializer));
        var bytes = serializer.ListToExcelByteArray(list);
        return File(bytes, ExcelMime);
    }

    public IActionResult Excel<T>(List<T> list, string fileName)
    {
        var serializer = (IExcelSerializer)HttpContext.RequestServices.GetRequiredService(typeof(IExcelSerializer));
        var bytes = serializer.ListToExcelByteArray(list);
        return File(bytes, ExcelMime, $"{fileName}.xlsx");
    }

    protected async Task<IActionResult> Create<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command, cancellationToken);
        return result.Status == ApplicationServiceStatus.Ok 
            ? StatusCode((int)HttpStatusCode.Created, result.Data) 
            : BadRequest(result.Messages);
    }

    protected async Task<IActionResult> Create<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command, cancellationToken);
        return result.Status == ApplicationServiceStatus.Ok
            ? StatusCode((int)HttpStatusCode.Created)
            : BadRequest(result.Messages);
    }

    protected async Task<IActionResult> Edit<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command, cancellationToken);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.OK, result.Data),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Edit<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command, cancellationToken);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.OK),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }


    protected async Task<IActionResult> Delete<TCommand, TCommandResult>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand<TCommandResult>
    {
        var result = await CommandDispatcher.Send<TCommand, TCommandResult>(command, cancellationToken);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.NoContent, result.Data),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Delete<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand
    {
        var result = await CommandDispatcher.Send(command, cancellationToken);
        return result.Status switch
        {
            ApplicationServiceStatus.Ok => StatusCode((int)HttpStatusCode.NoContent),
            ApplicationServiceStatus.NotFound => StatusCode((int)HttpStatusCode.NotFound, command),
            _ => BadRequest(result.Messages)
        };
    }

    protected async Task<IActionResult> Query<TQuery, TQueryResult>(TQuery query, CancellationToken cancellationToken) where TQuery : class, IQuery<TQueryResult>
    {
        var result = await QueryDispatcher.Execute<TQuery, TQueryResult>(query, cancellationToken);
        if (result.Status == ApplicationServiceStatus.NotFound || result.Data == null)
            return StatusCode((int)HttpStatusCode.NoContent);
        if (result.Status == ApplicationServiceStatus.Ok)
            return Ok(result.Data);

        return BadRequest(result.Messages);
    }
}