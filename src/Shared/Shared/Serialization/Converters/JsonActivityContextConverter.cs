using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FeedTracker.Shared.Serialization.Converters
{
    internal sealed class JsonActivityContextConverter : JsonConverter<ActivityContext>
    {
        public override ActivityContext Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<string> values = new();
            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.String or JsonTokenType.Null)
                    values.Add(reader.GetString()!);
            }

            return new ActivityContext(
                traceId: ActivityTraceId.CreateFromString(values[0]),
                spanId: ActivitySpanId.CreateFromString(values[1]),
                traceFlags: Enum.Parse<ActivityTraceFlags>(values[2]),
                traceState: values[3],
                isRemote: values[4] == "true");
        }

        public override void Write(Utf8JsonWriter writer, ActivityContext value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("traceId", value.TraceId.ToHexString());
            writer.WriteString("spanId", value.SpanId.ToHexString());
            writer.WriteString("traceFlags", value.TraceFlags.ToString());
            writer.WriteString("traceState", value.TraceState);
            writer.WriteString("isRemote", value.IsRemote ? "true" : "false");
            writer.WriteEndObject();
        }
    }
}
