using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace FunctionCalling.OpenAI.Helpers;

internal sealed class FunctionInvoker : IFunctionInvoker
{
    public object? InvokeMethodFromResponse(Func<object> typeFactory, string methodName, string arguments)
    {
        var instance = typeFactory();
        var type = instance.GetType();
        var method = type.GetMethod(methodName);
        if (method is null)
            throw new ArgumentNullException($"Could not find method {methodName} on type {type.Name}");

        var functionArguments = ParseArguments(arguments);

        var parameters = method.GetParameters();
        List<object?> args = MapArguments(parameters, functionArguments);

        var result = method.Invoke(instance, args.ToArray());
        if (result is null)
            throw new ArgumentNullException($"Method {methodName} on type {type.Name} returned null");

        return result;
    }

    private static List<object?> MapArguments(ParameterInfo[] parameters, List<KeyValuePair<string, string>> functionArguments)
    {
        var args = new List<object?>();
        foreach (var parameter in parameters)
        {
            var argument = functionArguments.FirstOrDefault(a => a.Key == parameter.Name);
            var value = ParseStringToType(argument.Value, parameter.ParameterType);
            args.Add(value);
        }

        return args;
    }

    private static List<KeyValuePair<string, string>> ParseArguments(string arguments)
    {
        var functionsJsonObj = JObject.Parse(arguments);
        var functionArguments = new List<KeyValuePair<string, string>>();
        foreach (var param in functionsJsonObj.Children())
        {
            functionArguments.Add(new KeyValuePair<string, string>(param.Path, param.First!.ToString()));
        }

        return functionArguments;
    }

    public static object? ParseStringToType(string? value, Type targetType)
    {
        if (value is null)
            return null;

        if (targetType is null)
            throw new ArgumentNullException(nameof(targetType));

        // If the target type is a string, return the value directly.
        if (targetType == typeof(string))
            return value;

        // Use TypeDescriptor to get a converter for the target type.
        var converter = TypeDescriptor.GetConverter(targetType);

        if (converter == null || !converter.CanConvertFrom(typeof(string)))
        {
            throw new InvalidOperationException($"No valid converter for type {targetType.FullName}");
        }

        try
        {
            return converter.ConvertFromString(value);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to convert '{value}' to {targetType.FullName}", ex);
        }
    }
}
