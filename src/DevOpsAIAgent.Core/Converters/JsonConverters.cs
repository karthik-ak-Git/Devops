using System.Text.Json;
using System.Text.Json.Serialization;
using Pgvector;

namespace DevOpsAIAgent.Core.Converters;

/// <summary>
/// Custom JSON converter for Pgvector.Vector type
/// </summary>
public class VectorJsonConverter : JsonConverter<Vector?>
{
    public override Vector? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected array for Vector conversion");

        var values = new List<float>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                break;

            if (reader.TokenType == JsonTokenType.Number)
            {
                values.Add(reader.GetSingle());
            }
            else
            {
                throw new JsonException("Expected number in Vector array");
            }
        }

        return new Vector(values.ToArray());
    }

    public override void Write(Utf8JsonWriter writer, Vector? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        // Convert Vector to array for serialization
        var floatArray = value.ToArray();
        foreach (var element in floatArray)
        {
            writer.WriteNumberValue(element);
        }
        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON serializer options configured for the DevOpsAIAgent application
/// </summary>
public static class JsonSerializerOptionsProvider
{
    /// <summary>
    /// Default JSON serializer options for the application
    /// </summary>
    public static JsonSerializerOptions DefaultOptions { get; } = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        // Add custom converters
        options.Converters.Add(new VectorJsonConverter());
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        return options;
    }
}