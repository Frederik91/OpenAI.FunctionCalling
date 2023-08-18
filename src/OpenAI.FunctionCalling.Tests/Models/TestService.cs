using OpenAI.FunctionCalling.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.FunctionCalling.Tests.Models;
internal sealed class TestService
{
    [Function("Get test data")]
    public TestResult GetTestData([Parameter("Get test data with content")] string data, TestOptions? option = null)
    {
        return new TestResult(data, option);
    }
}
