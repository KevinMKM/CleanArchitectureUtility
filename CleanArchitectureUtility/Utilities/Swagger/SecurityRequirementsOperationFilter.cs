using CleanArchitectureUtility.Utilities.Swagger.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanArchitectureUtility.Utilities.Swagger;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    private readonly SwaggerOption _option;

    public SecurityRequirementsOperationFilter(SwaggerOption option)
    {
        _option = option;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security ??= new List<OpenApiSecurityRequirement>();
        _option.EnabledSecurities.ForEach(security =>
        {
            operation.Security.Add(security.ToOpenApiSecurityRequirement());
        });
    }
}