using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IrisPayments.Helpers.Swagger;
public class SwaggerHeaderFilter : IOperationFilter {

    private readonly IConfiguration _config;

    public SwaggerHeaderFilter(IConfiguration config) {
        _config = config;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if(operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();
        var actionName = (context.ApiDescription.ActionDescriptor as ControllerActionDescriptor)?.ActionName;

        if(!string.IsNullOrWhiteSpace(actionName) && !actionName.Equals("PaymentRequest")) {
            operation.Parameters.Add(new OpenApiParameter {
                Name = _config.GetValue<string>("AppSettings:AppApiKeyHeaderName"),
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema {
                    Type = "string"
                }
            });
        }
    }
}