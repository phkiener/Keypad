using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keypad.Configuration.Converters;

public class KeypadKeyConverter : JsonConverter<KeypadKey>
{
    public override KeypadKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, KeypadKey value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
