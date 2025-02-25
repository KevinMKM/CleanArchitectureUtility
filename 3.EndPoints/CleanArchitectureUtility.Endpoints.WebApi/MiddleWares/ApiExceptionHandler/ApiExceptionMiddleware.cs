﻿using System.Net;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Endpoints.WebApi.MiddleWares.ApiExceptionHandler;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly ApiExceptionOptions _options;
    private readonly IJsonSerializer _serializer;
    private readonly ITranslator _translator;

    public ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next, ILogger<ApiExceptionMiddleware> logger, IJsonSerializer serializer, ITranslator translator)
    {
        _next = next;
        _logger = logger;
        _options = options;
        _serializer = serializer;
        _translator = translator;
    }

    public async Task Invoke(HttpContext context, IScopeInformation scopeInfo /* other dependencies */)
    {
        using var hostScope = _logger.BeginScope(scopeInfo.HostScopeInfo);
        using var requestScope = _logger.BeginScope(scopeInfo.RequestScopeInfo);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
        finally
        {
            hostScope.Dispose();
            requestScope.Dispose();
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new ApiError
        {
            Id = Guid.NewGuid().ToString(),
            Status = (short)HttpStatusCode.InternalServerError,
            Title = _translator["SOME_KIND_OF_ERROR_OCCURRED_IN_THE_API"]
        };

        _options.AddResponseDetails?.Invoke(context, exception, error);
        var innerExMessage = GetInnermostExceptionMessage(exception);
        var level = _options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;
        _logger.Log(level, exception, $"BADNESS!!! {innerExMessage} -- {error.Id}.");
        var result = _serializer.Serialize(error);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(result);
    }
    private static string GetInnermostExceptionMessage(Exception exception) 
        => exception.InnerException != null 
            ? GetInnermostExceptionMessage(exception.InnerException) 
            : exception.Message;
}