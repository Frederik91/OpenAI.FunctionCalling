using Azure.AI.OpenAI;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Reflection;
using OpenAI.FunctionCalling.Attributes;
using OpenAI.FunctionCalling.Models;
using OpenAI.FunctionCalling.Extensions;

namespace OpenAI.FunctionCalling.Helpers;
internal sealed class FunctionParser : IFunctionParser
{
    public FunctionDefinition Parse<T>(string methodName) where T : class
    {
        var instanceType = typeof(T);
        return Parse(instanceType, methodName);
    }

    private static FunctionDefinition Parse(Type instanceType, string methodName)
    {
        var method = instanceType.GetMethod(methodName);
        if (method is null)
            throw new ArgumentNullException($"Method {methodName} not found on type {instanceType.Name}");

        var functionAttribute = method.GetFunctionAttribute();

        var properties = GetProperties(method);

        var jsonObject = new JsonObject
        {
            { "type", "object" },
            { "properties", properties.Properties },
            { "required", properties.Required }
        };

        var json = jsonObject.ToJsonString(new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return new FunctionDefinition
        {
            Name = instanceType.Name + "-" + methodName,
            Description = functionAttribute.Description,
            Parameters = BinaryData.FromString(json)
        };
    }

    private static FunctionProperties GetProperties(MethodInfo method)
    {
        var props = new JsonObject();
        var required = new JsonArray();
        foreach (var parameter in method.GetParameters())
        {
            var data = GetParameterData(parameter);
            if (!parameter.HasDefaultValue)
                required.Add(parameter.Name!);

            var parameterJson = new JsonObject();
            if (data.Description != null)
                parameterJson.Add("Description", data.Description);
            if (data.Enum != null)
            {
                var array = new JsonArray();
                foreach (var item in data.Enum)
                {
                    array.Add(item);
                }
                parameterJson.Add("Enum", array);
            }
            parameterJson.Add("Type", data.Type);

            props.Add(parameter.Name!, parameterJson);
        }
        return new FunctionProperties(props, required);
    }


    private static ParameterData GetParameterData(ParameterInfo parameter)
    {
        var type = parameter.ParameterType;
        if (Nullable.GetUnderlyingType(parameter.ParameterType) is { } nullableType)
            type = nullableType;

        var parameterDescription = parameter.GetParameterAttribute();

        var enums = GetEnumValues(parameter, type);

        var data = new ParameterData(type.Name, parameterDescription?.Description, enums);
        return data;
    }

    private static List<string>? GetEnumValues(ParameterInfo parameter, Type type)
    {
        List<string>? enums = null;
        if (type.IsEnum)
        {
            Type enumType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
            var enumValues = enumType.GetEnumValues();
            enums = new List<string>();
            foreach (var enumValue in enumValues)
            {
                enums.Add(enumValue.ToString()!);
            }
        }

        return enums;
    }
}
