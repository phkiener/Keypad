using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keypad.Configuration.Converters;

public class KeypadButtonConverter : JsonConverter<KeypadButton>
{
    public override KeypadButton Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, KeypadButton value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
