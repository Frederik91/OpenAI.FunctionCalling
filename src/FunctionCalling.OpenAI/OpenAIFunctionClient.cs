using Azure.AI.OpenAI;
using FunctionCalling.Helpers;
using FunctionCalling.OpenAI.Helpers;
using System.Text.Json;

namespace FunctionCalling.OpenAI;

public record ParameterData(string Type, string? Description, IEnumerable<string>? Enum);

public class OpenAIFunctionClient : IFunctionClient
{
    private readonly Dictionary<string, Func<object>> _functionTypes = new();
    private readonly IOpenAIClientWrapper _openAIClient;
    private readonly IFunctionParser _functionParser;
    private readonly IFunctionInvoker _functionInvoker;
    private readonly ChatCompletionsOptions _options = new();
    private readonly string _deploymentOrModelName;

    public OpenAIFunctionClient(OpenAIClient openAIClient, string deploymentOrModelName) : this(new OpenAIClientWrapper(openAIClient), new FunctionParser(), new FunctionInvoker()) 
    {
        _deploymentOrModelName = deploymentOrModelName;
    }

    internal OpenAIFunctionClient(IOpenAIClientWrapper openAIClient, IFunctionParser functionParser, IFunctionInvoker functionInvoker)
    {
        _openAIClient = openAIClient;
        _functionParser = functionParser;
        _functionInvoker = functionInvoker;
        _deploymentOrModelName = "test";
    }

    public async Task<string> Query(string query)
    {
        _options.Messages.Add(new ChatMessage(ChatRole.User, query));
        var response = await _openAIClient.GetChatCompletionsAsync(_deploymentOrModelName, _options);
        while (response.FinishReason == CompletionsFinishReason.FunctionCall)
        {
            // Include the FunctionCall message in the conversation history
            _options.Messages.Add(response.Message);

            var name = response.Message.FunctionCall.Name;
            var typeFactory = _functionTypes[name];
            var methodName = name.Split('-').Last();
            var result = _functionInvoker.InvokeMethodFromResponse(typeFactory, methodName, response.Message.FunctionCall.Arguments);

            var messageContent = JsonSerializer.Serialize(
                result, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var functionResponseMessage = new ChatMessage(ChatRole.Function, messageContent)
            {
                Name = response.Message.FunctionCall.Name
            };

            _options.Messages.Add(functionResponseMessage);

            response = await _openAIClient.GetChatCompletionsAsync(_deploymentOrModelName, _options);
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