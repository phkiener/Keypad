using System.Text.Json;
using System.Text.Json.Serialization;
using Keypad.Core.Abstractions;

namespace Keypad.Configuration.Converters;

internal sealed class DeviceImageConverter : JsonConverter<DeviceImage>
{
    public override DeviceImage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var imageSpecification = reader.GetString();
        if (string.IsNullOrWhiteSpace(imageSpecification))
        {
            throw new FormatException("Image specification cannot be empty.");
        }

        if (imageSpecification.StartsWith("color:"))
        {
            var color = imageSpecification["color:".Length..];
            return new DeviceImage.Color(color);
        }

        if (imageSpecification.StartsWith("file:"))
        {
            var path = imageSpecification["file:".Length..];
            return new DeviceImage.File(path);
        }
        
        throw new FormatException($"Invalid image specification '{imageSpecification}'.");
    }

    public override void Write(Utf8JsonWriter writer, DeviceImage value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }
}
