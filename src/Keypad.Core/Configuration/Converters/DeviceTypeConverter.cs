using System.Text.Json;
using System.Text.Json.Serialization;
using Keypad.Core.Abstractions;

namespace Keypad.Configuration.Converters;

internal sealed class DeviceTypeConverter : JsonConverter<DeviceType>
{
    public override DeviceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var type = reader.GetString();

        return type switch
        {
            "StreamDeck XL 2022" or "StreamDeckXL2022" or "XL2022" => DeviceType.StreamDeckXL2022,
            _ => throw new FormatException($"Unknown device type: '{type}'.")
        };
    }

    public override void Write(Utf8JsonWriter writer, DeviceType value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }
}
