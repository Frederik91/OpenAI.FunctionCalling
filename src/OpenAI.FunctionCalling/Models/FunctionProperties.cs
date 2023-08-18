using System.Text.Json.Nodes;

namespace OpenAI.FunctionCalling.Models;
internal sealed class FunctionProperties
{
    public JsonObject Properties { get; }
    public JsonArray Required { get; }
	public FunctionProperties(JsonObject properties, JsonArray required)
	{
        Properties = properties;
        Required = required;
    }

}
