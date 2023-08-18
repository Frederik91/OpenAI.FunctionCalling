using OpenAI.FunctionCalling.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OpenAI.FunctionCalling.Extensions;
internal static class AttributeExtensions
{
    internal static FunctionAttribute GetFunctionAttribute(this MethodInfo method)
        => method.GetType().GetCustomAttributes(typeof(FunctionAttribute), false).FirstOrDefault() as FunctionAttribute ?? throw new InvalidOperationException($"Method {method.Name} is missing a description. Add a {nameof(FunctionAttribute)} to the method");

    internal static ParameterAttribute? GetParameterAttribute(this ParameterInfo parameter)
    => parameter.GetType().GetCustomAttributes(typeof(ParameterAttribute), false).FirstOrDefault() as ParameterAttribute;
}
