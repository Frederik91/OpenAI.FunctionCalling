using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Reflection;

namespace FunctionCalling.OpenAI.Helpers;
public interface IFunctionInvoker
{
    public object? InvokeMethodFromResponse(Func<object> typeFactory, string methodName, string arguments);
}
