using OpenAI.FunctionCalling.Attributes;
using OpenAI.FunctionCalling.Helpers;
using OpenAI.FunctionCalling.Tests.Models;
using System.ComponentModel;

namespace OpenAI.FunctionCalling.Tests;

public class FunctionParserTests
{
    [Fact]
    public void Parse_UsingTestServiceForData_ReturnsValidTestData()
    {
        var result = new FunctionParser().Parse<TestService>(nameof(TestService.GetTestData));
        Assert.NotNull(result);
        Assert.Equal("Get test data", result.Description);
        var parametersJson = result.Parameters.ToString();
        Assert.Contains("data", parametersJson);
        Assert.Contains("Get test data with content", parametersJson);
        Assert.Contains("option", parametersJson);
    }
}