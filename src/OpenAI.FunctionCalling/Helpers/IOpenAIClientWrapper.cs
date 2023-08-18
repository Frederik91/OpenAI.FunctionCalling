using Azure.AI.OpenAI;
using OpenAI.FunctionCalling.Models;

namespace OpenAI.FunctionCalling.Helpers;
public interface IOpenAIClientWrapper
{
    Task<ChatCompletionResponse> GetChatCompletionsAsync(string deploymentOrModelName, ChatCompletionsOptions options);
}
