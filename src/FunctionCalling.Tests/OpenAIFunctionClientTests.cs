using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Xunit;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using FunctionCalling.Helpers;
using FunctionCalling;
using FunctionCalling.Models;

namespace FunctionCalling.Tests;

public class OpenAIFunctionClientTests
{
    [Fact]
    public async Task Query_WhenFinishReasonIsNotFunctionCall_ReturnsContent()
    {
        // Arrange
        var client = Substitute.For<IOpenAIClientWrapper>();
        var parser = Substitute.For<IFunctionParser>();
        var invoker = Substitute.For<IFunctionInvoker>();
        var cut = new OpenAIFunctionClient(client, parser, invoker);
        var query = "Test query";
        var methodName = "GetTestData";
        var functionCallMessage = new ChatMessage(ChatRole.Assistant, string.Empty)
        {
            FunctionCall = new FunctionCall(methodName, string.Empty)
        };
        client.GetChatCompletionsAsync(Arg.Any<string>(), Arg.Is<ChatCompletionsOptions>(o => o.Messages.Any(m => m.Content == query)))
                    .Returns(Task.FromResult(new ChatCompletionResponse
                    {
                        FinishReason = CompletionsFinishReason.FunctionCall,
                        Message = functionCallMessage
                    }));

        var finishedMessage = new ChatMessage(ChatRole.Assistant, string.Empty)
        {
            Content = "Finished"
        };
        client.GetChatCompletionsAsync(Arg.Any<string>(), Arg.Is<ChatCompletionsOptions>(o => o.Messages.Any(m => m.Content == query)))
            .Returns(Task.FromResult(new ChatCompletionResponse
            {
                FinishReason = CompletionsFinishReason.Stopped,
                Message = finishedMessage
            }));

        // Act
        var result = await cut.Query(query);

        // Assert
        Assert.Equal(finishedMessage.Content, result);
    }
}
