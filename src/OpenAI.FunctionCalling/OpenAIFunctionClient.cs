using Azure.AI.OpenAI;
using OpenAI.FunctionCalling.Helpers;
using System.Text.Json;

namespace OpenAI.FunctionCalling;

public record ParameterData(string Type, string? Description, IEnumerable<string>? Enum);

public class OpenAIFunctionClient : IOpenAIFunctionClient
{
    private readonly Dictionary<string, Func<object>> _functionTypes = new();
    private readonly IOpenAIClientWrapper _openAIClient;
    private readonly IFunctionParser _functionParser;
    private readonly IFunctionInvoker _functionInvoker;
    private readonly ChatCompletionsOptions _options = new();

    public OpenAIFunctionClient(OpenAIClient openAIClient)
    {
        _openAIClient = new OpenAIClientWrapper(openAIClient);
        _functionParser = new FunctionParser();
        _functionInvoker = new FunctionInvoker();
    }

    internal OpenAIFunctionClient(IOpenAIClientWrapper openAIClient, IFunctionParser functionParser, IFunctionInvoker functionInvoker)
    {
        _openAIClient = openAIClient;
        _functionParser = functionParser;
        _functionInvoker = functionInvoker;
    }

    public async Task<string> Query(string query)
    {
        _options.Messages.Add(new ChatMessage(ChatRole.User, query));
        var response = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo-0613", _options);
        while (response.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            // Include the FunctionCall message in the conversation history
            _options.Messages.Add(response.Message);

            var name = response.Message.FunctionCall.Name;
            var typeFactory = _functionTypes[name];
            var methodName = name.Split('-').Last();
            var result = _functionInvoker.InvokeMethodFromResponse(typeFactory, methodName, response.Message.FunctionCall.Arguments);

            var functionResponseMessage = new ChatMessage(
                ChatRole.Function,
                JsonSerializer.Serialize(
                    result,
                    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
            functionResponseMessage.Name = response.Message.FunctionCall.Name;

            _options.Messages.Add(functionResponseMessage);

            response = await _openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo-0613", _options);
            continue;
        }
        return response.Message.Content;
    }

    public void RegisterFunction<T>(string methodName, Func<T> factory) where T : class
    {
        var functionDefinition = _functionParser.Parse<T>(methodName);

        _options.Functions.Add(functionDefinition);
        _functionTypes.Add(functionDefinition.Name, factory);
    }
}