using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedTracker.Shared.Serialization
{
    internal sealed class SystemTextJsonSerializer : ISerializer
    {
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = 
            { 
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new ActivityContextSerializer()
            }
        };

        public T? Deserialize<T>(string value)
            => JsonSerializer.Deserialize<T>(value, options);

        public string Serialize<T>(T value)
            => JsonSerializer.Serialize(value, options);
    }
}
