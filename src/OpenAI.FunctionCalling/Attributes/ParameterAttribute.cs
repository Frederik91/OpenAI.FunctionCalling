using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FunctionCalling.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ParameterAttribute : Attribute
{
    public string? Description { get; init; }
}
