using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace OpenAI.FunctionCalling.Helpers;
public interface IFunctionInvoker
{
    public object? InvokeMethodFromResponse(Func<object> typeFactory, string methodName, string arguments);
}
