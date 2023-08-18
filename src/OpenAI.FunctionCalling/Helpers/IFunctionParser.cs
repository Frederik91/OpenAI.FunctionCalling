using Azure.AI.OpenAI;

namespace OpenAI.FunctionCalling.Helpers;
public interface IFunctionParser
{
    public FunctionDefinition Parse<T>(string methodName) where T : class;
}
