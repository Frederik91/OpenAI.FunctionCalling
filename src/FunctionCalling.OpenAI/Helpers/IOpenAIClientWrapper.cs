using Azure.AI.OpenAI;
using FunctionCalling.Models;

namespace FunctionCalling.OpenAI.Helpers;
public interface IOpenAIClientWrapper
{
    Task<ChatCompletionResponse> GetChatCompletionsAsync(string deploymentOrModelName, ChatCompletionsOptions options);
}
