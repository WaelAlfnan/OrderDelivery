using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OrderDelivery.Api.Extensions
{
    /// <summary>
    /// Represents the Swagger/Swashbuckle schema filter used to handle problematic types.
    /// </summary>
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies the filter to the specified schema using the given context.
        /// </summary>
        /// <param name="schema">The schema to apply the filter to.</param>
        /// <param name="context">The current schema filter context.</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context.Type == null)
            {
                return;
            }

            var excludedProperties = context.Type.GetProperties()
                .Where(prop => prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any());

            foreach (var excludedProperty in excludedProperties)
            {
                var propertyName = char.ToLowerInvariant(excludedProperty.Name[0]) + excludedProperty.Name[1..];
                schema.Properties.Remove(propertyName);
            }
        }
    }
}
