using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Endpoints.API.MiddleWares.ApiExceptionHandler
{
    public class ApiExceptionOptions
    {
        public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }
        public Func<Exception, LogLevel> DetermineLogLevel { get; set; }
    }
}