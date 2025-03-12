using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApexWebFileMgr
{


    public class SwaggerFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody == null)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>()
                };
            }

            if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
            {
                operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>()
                    }
                };
            }

            var schemaProperties = operation.RequestBody.Content["multipart/form-data"].Schema.Properties;

            foreach (var param in context.MethodInfo.GetParameters())
            {
                if (param.ParameterType == typeof(IFormFile))
                {
                    schemaProperties["file"] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                }
                else
                {
                    schemaProperties[param.Name] = new OpenApiSchema
                    {
                        Type = "string"
                    };
                }
            }
        }
    }



}
