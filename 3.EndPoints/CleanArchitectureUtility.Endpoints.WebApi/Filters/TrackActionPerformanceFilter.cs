﻿using System.Diagnostics;
using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Endpoints.WebApi.Filters;

public partial class TrackActionPerformanceFilter : IActionFilter
{
    private Stopwatch _timer;
    private readonly ILogger<TrackActionPerformanceFilter> _logger;
    private readonly IScopeInformation _scopeInfo;
    private IDisposable _userScope;
    private IDisposable _hostScope;
    private IDisposable _requestScope;

    public TrackActionPerformanceFilter(ILogger<TrackActionPerformanceFilter> logger, IScopeInformation scopeInfo)
    {
        _logger = logger;
        _scopeInfo = scopeInfo;
    }
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _timer = new Stopwatch();

        var userDict = new Dictionary<string, string>
            {
                { "UserId", context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "sub")?.Value },
                { "OAuth2 Scopes", string.Join(",", context.HttpContext.User.Claims.Where(c => c.Type == "scope")?.Select(c => c.Value)) }
            };
        _userScope = _logger.BeginScope(userDict);
        _hostScope = _logger.BeginScope(_scopeInfo.HostScopeInfo);
        _requestScope = _logger.BeginScope(_scopeInfo.RequestScopeInfo);

        _timer.Start();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _timer.Stop();
        if (context.Exception == null)
        {
            var message = $"{context.HttpContext.Request.Path} {context.HttpContext.Request.Method} code took {_timer.ElapsedMilliseconds}.";
            _logger.LogInformation(0, message);
        }

        _userScope?.Dispose();
        _hostScope?.Dispose();
        _requestScope.Dispose();
    }
}