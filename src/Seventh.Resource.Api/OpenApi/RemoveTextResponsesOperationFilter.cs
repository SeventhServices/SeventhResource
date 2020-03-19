using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Seventh.Resource.Api.OpenApi
{
    public class RemoveTextResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var response in operation.Responses)
            {
                response.Value.Content.Remove("text/plain");
            }
        }
    }
}
