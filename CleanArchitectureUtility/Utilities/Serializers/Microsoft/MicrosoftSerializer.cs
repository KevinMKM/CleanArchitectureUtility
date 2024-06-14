using System.Text.Json;
using CleanArchitectureUtility.Core.Abstractions.Serializers;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Utilities.Serializers.Microsoft;

public class MicrosoftSerializer : IJsonSerializer, IDisposable
{
    private readonly ILogger<MicrosoftSerializer> _logger;
    private readonly JsonSerializerOptions? _options= new() { PropertyNameCaseInsensitive = true };

public MicrosoftSerializer(ILogger<MicrosoftSerializer> logger)
    {
        _logger = logger;
        _logger.LogInformation("Microsoft Serializer Start working");
    }

    public TOutput? Deserialize<TOutput>(string input)
    {
        _logger.LogTrace($"Microsoft Serializer Deserialize with name {input}");
        return string.IsNullOrWhiteSpace(input) 
            ? default 
            : JsonSerializer.Deserialize<TOutput>(input, _options);
    }

    public object? Deserialize(string input, Type type)
    {
        _logger.LogTrace($"Microsoft Serializer Deserialize with name {input} and type {type}");
        return string.IsNullOrWhiteSpace(input) 
            ? default 
            : JsonSerializer.Deserialize(input, type, _options);
    }

    public string Serialize<TInput>(TInput input)
    {
        _logger.LogTrace($"Microsoft Serializer Serialize with name {input}");
        return input == null ? string.Empty : JsonSerializer.Serialize(input, _options);
    }

    public void Dispose() => _logger.LogInformation("Microsoft Serializer Stop working");
}
