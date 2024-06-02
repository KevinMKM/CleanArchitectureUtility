using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitectureUtility.Endpoints.API.Filters;

public partial class TrackActionPerformanceFilter
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;
            var errors = context.ModelState
                .Where(x => x.Value is not null && x.Value.Errors.Any())
                .Select(kvp => string.Join(", ", kvp.Value?.Errors.Select(p => p.ErrorMessage) ?? Array.Empty<string>()))
                .ToList();
            context.Result = new BadRequestObjectResult(errors);
        }
    }
}