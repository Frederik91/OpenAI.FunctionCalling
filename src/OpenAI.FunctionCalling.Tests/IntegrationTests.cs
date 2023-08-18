using Azure.AI.OpenAI;
using OpenAI.FunctionCalling.Attributes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.FunctionCalling.Tests;
public class IntegrationTests
{
    [Fact(Skip = "Integration test")]
    public async Task Query_SetupWeatherRequest_GetWeatherResponse()
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<IntegrationTests>().Build();
        var client = new OpenAIClient(configuration["OpenAI:ApiKey"]);

        var functionClient = new OpenAIFunctionClient(client);
        functionClient.RegisterFunction(nameof(WeatherService.GetWeatherForLocation), () => new WeatherService());

        var query = "What is the weather in Oslo in Celsius?";

        var result = await functionClient.Query(query);

        Assert.Contains("15", result);
        Assert.Contains("Oslo", result);
        Assert.Contains("Celsius", result);
    }
    private class WeatherService
    {
        [Function(Description = "Get the current weather in a given location")]
        public WeatherResult GetWeatherForLocation([Parameter(Description = "Location to get the current weather from")] string location, [Parameter(Description = "Temperature unit")] Unit unit)
        {
            return new WeatherResult(15, unit);
        }
    }

    private sealed record WeatherResult(double Temperature, Unit unit);

    private enum Unit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }
}
