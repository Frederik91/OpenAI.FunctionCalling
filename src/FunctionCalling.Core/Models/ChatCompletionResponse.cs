using Azure.AI.OpenAI;

namespace FunctionCalling.Core.Models;
public sealed class ChatCompletionResponse
{
    public required CompletionsFinishReason FinishReason { get; init; }
    public required ChatMessage Message { get; init; }
}
