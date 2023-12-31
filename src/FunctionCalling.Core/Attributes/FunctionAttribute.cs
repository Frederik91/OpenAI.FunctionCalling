﻿using System.Diagnostics.CodeAnalysis;

namespace FunctionCalling.Core.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed partial class FunctionAttribute : Attribute
{
    public string? Description { get; init; }
}