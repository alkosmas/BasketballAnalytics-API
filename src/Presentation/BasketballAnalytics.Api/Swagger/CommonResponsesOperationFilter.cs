// BasketballAnalytics.Api/Swagger/CommonResponsesOperationFilter.cs
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BasketballAnalytics.Api.Swagger;

public class CommonResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request - Validation errors"
            });
        }

        if (!operation.Responses.ContainsKey("409"))
        {
            operation.Responses.Add("409", new OpenApiResponse
            {
                Description = "Conflict - Resource already exists"
            });
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error"
            });
        }
    }
}