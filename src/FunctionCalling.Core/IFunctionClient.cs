namespace FunctionCalling.Core;
public interface IFunctionClient
{
    Task<string> Query(string query);
    void RegisterFunction<T>(string methodName, Func<T> factory) where T : class;
}
