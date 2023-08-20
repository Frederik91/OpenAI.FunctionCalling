using FunctionCalling.Attributes;
using FunctionCalling.Helpers;
using FunctionCalling.Tests.Models;
using System.ComponentModel;

namespace FunctionCalling.Tests;

public class FunctionInvokerTests
{
    [Fact]
    public void InvokeMethodFromResponse_UsingGetTestData_ReturnsValidTestResult()
    {
        var result = new FunctionInvoker().InvokeMethodFromResponse(() => new TestService(), nameof(TestService.GetTestData), "{\n  \"data\": \"Test\"\n}");
        Assert.NotNull(result);
        var weatherResult = Assert.IsType<TestResult>(result);
        Assert.Equal("Test", weatherResult.Data);
    }
}