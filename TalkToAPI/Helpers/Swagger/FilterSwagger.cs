using Microsoft.AspNetCore.JsonPatch.Operations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using TalkToAPI.Helpers.Swagger;

namespace TalkToAPI.Helpers.Swagger
{
    public class FilterSwagger : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionApiVersionModel = context.ApiDescription.ActionDescriptor?.GetApiVersion();
            if (actionApiVersionModel == null)
            {
                return;
            }
        }
    }
}

