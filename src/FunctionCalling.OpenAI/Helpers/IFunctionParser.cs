using Azure.AI.OpenAI;

namespace FunctionCalling.OpenAI.Helpers;
public interface IFunctionParser
{
    public FunctionDefinition Parse<T>(string methodName) where T : class;
}
