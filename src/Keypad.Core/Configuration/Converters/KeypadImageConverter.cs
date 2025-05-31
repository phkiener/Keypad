using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keypad.Configuration.Converters;

public sealed class KeypadImageConverter : JsonConverter<KeypadImage>
{
    public override KeypadImage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var imageSpecification = reader.GetString();
        if (string.IsNullOrWhiteSpace(imageSpecification))
        {
            throw new FormatException("Image specification cannot be empty.");
        }

        if (imageSpecification.StartsWith("color:"))
        {
            var color = imageSpecification["color:".Length..];
            return new KeypadImage.Color(color);
        }

        if (imageSpecification.StartsWith("file:"))
        {
            var path = imageSpecification["file:".Length..];
            return new KeypadImage.File(path);
        }
        
        throw new FormatException($"Invalid image specification '{imageSpecification}'");
    }

    public override void Write(Utf8JsonWriter writer, KeypadImage value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }
}
