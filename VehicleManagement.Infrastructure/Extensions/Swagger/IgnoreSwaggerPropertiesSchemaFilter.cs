using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VehicleManagement.Infrastructure.Extensions.Swagger;

public class IgnoreSwaggerPropertiesSchemaFilter : ISchemaFilter
{
    private static readonly string[] IgnoredProps =
    [
        "Filter",
        "OrderBy"
    ];

    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var prop in IgnoredProps)
        {
            if (schema.Properties?.ContainsKey(prop) == true)
            {
                schema.Properties.Remove(prop);
            }
        }
    }
}
