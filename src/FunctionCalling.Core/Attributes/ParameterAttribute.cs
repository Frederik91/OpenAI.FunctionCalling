using System.Diagnostics.CodeAnalysis;

namespace FunctionCalling.Core.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ParameterAttribute : Attribute
{
    public string? Description { get; init; }
}
