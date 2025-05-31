using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keypad.Configuration.Converters;

public class KeypadImageConverter : JsonConverter<KeypadImage>
{
    public override KeypadImage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, KeypadImage value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
