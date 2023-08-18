# OpenAI.FunctionCalling

An easy-to-use C# library that wraps the OpenAI function call API, allowing developers to seamlessly set up and invoke a sequence of methods directly via OpenAI. Use attributes to configure classes, and let `OpenAI.FunctionCalling` manage the API calls, response evaluation, type mapping, and method invocation.

## Features

- Intuitive integration with the OpenAI API.
- Attribute-based configuration for classes.
- Simplified API calls, response evaluation, and method invocations.
- Efficient type and argument mapping.

## Installation

Using NuGet Package Manager:

```shell
Install-Package OpenAI.FunctionCalling
```


```csharp
var client = new OpenAIClient(configuration["my-api-key"]);

var functionClient = new OpenAIFunctionClient(client);
functionClient.RegisterFunction(
    methodName: nameof(WeatherService.GetWeatherForLocation), 
    factory: () => new WeatherService());

var query = "What is the weather in Oslo in Celsius?";

var result = await functionClient.Query(query);
Console.WriteLine(result); // The weather in Oslo is 10 degrees Celsius
```

WeatherService:
```csharp
private class WeatherService
{
    [Function(Description = "Get the current weather in a given location")]
    public WeatherResult GetWeatherForLocation(
        [Parameter(Description = "Location to get the current weather from")] string location, 
        [Parameter(Description = "Temperature unit")] Unit unit)
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
```

## Abstracting Function Calling with OpenAI.FunctionCalling
With the emergence of Function Calling capabilities in models like gpt-4-0613 and gpt-3.5-turbo-0613, developers now have the power to describe specific functions to the model and obtain structured JSON responses to interact with external tools and APIs. This feature enables the transformation of natural language into actionable API calls, structured data extraction, and more. However, the integration process involves understanding new API parameters, setting up function descriptors using JSON Schema, handling responses, and more.

The OpenAI.FunctionCalling library abstracts away this complexity. Instead of manually crafting function descriptors and deciphering JSON outputs, developers can now effortlessly set up classes and methods using intuitive attributes. The library manages the intricate details of interacting with OpenAI's API, from constructing the necessary function calls to processing the model's structured responses. By leveraging OpenAI.FunctionCalling, you can focus on the logic and functionality you wish to implement, leaving the heavy lifting of API communication and data mapping to the library.