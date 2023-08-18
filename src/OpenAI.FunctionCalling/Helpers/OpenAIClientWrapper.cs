using Azure.AI.OpenAI;
using OpenAI.FunctionCalling.Models;

namespace OpenAI.FunctionCalling.Helpers;
internal sealed class OpenAIClientWrapper : IOpenAIClientWrapper
{
    private readonly OpenAIClient _client;

    public OpenAIClientWrapper(OpenAIClient client)
	{
        _client = client;
    }

    public async Task<ChatCompletionResponse> GetChatCompletionsAsync(string deploymentOrModelName, ChatCompletionsOptions options)
    {
        var response = await _client.GetChatCompletionsAsync(deploymentOrModelName, options);
        var choice = response.Value.Choices.First();
        return new ChatCompletionResponse()
        {
            FinishReason = choice.FinishReason,
            Message = choice.Message
        };
    }
}
