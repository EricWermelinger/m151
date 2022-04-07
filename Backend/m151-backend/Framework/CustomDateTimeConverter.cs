using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace m151_backend.Framework;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string FormatFrontend = "yyyy-MM-ddTHH:mm:ss.fffZ";
    private readonly string FormatBackend = "yyyy-MM-ddTHH:mm:ss.fff";

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    {
        writer.WriteStringValue(date.ToString(FormatFrontend));
    }
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string input = reader.GetString();
        if (input != null && input[^1] == 'Z')
        {
            input = input.Substring(0, input.Length - 1);
        }
        return DateTime.ParseExact(input, FormatBackend, null);
    }
}
