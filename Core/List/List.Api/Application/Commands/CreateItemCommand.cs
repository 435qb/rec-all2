using System.Text.Json.Nodes;
using MediatR;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using RecAll.Infrastructure;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RecAll.Core.List.Api.Application.Commands;

[SwaggerSchemaFilter(typeof(CreateItemCommandFilter))]
public class CreateItemCommand : IRequest<ServiceResult> {
    public int SetId { get; set; }

    public JsonObject CreateContribJson { get; set; }

    public CreateItemCommand(int setId, JsonObject createContribJson) {
        SetId = setId;
        CreateContribJson = createContribJson;
    }
}
internal class CreateItemCommandFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            [ "setId" ] = new OpenApiInteger(1),
            [ "createContribJson" ] = new OpenApiObject
            {
                [ "content" ] = new OpenApiString("题目"),
                [ "maskedcontent" ] = new OpenApiString("答案"),
            },
        };
    }
}