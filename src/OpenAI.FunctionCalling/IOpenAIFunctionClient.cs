using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.FunctionCalling;
public interface IOpenAIFunctionClient
{
    Task<string> Query(string query);
    void RegisterFunction<T>(string methodName, Func<T> factory) where T : class;
}
