using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Keypad.Core.Abstractions;

namespace Keypad.Configuration.Converters;

internal sealed partial class DeviceButtonConverter : JsonConverter<DeviceButton>
{
    public override DeviceButton Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var specification = reader.GetString();

        var match = SpecificationRegex().Match(specification ?? string.Empty);
        if (!match.Success)
        {
            throw new FormatException($"Invalid button specification '{specification}'.");
        }
        
        var row = int.Parse(match.Groups[1].Value);
        var column = int.Parse(match.Groups[2].Value);

        return new DeviceButton(row, column);
    }

    public override void Write(Utf8JsonWriter writer, DeviceButton value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }

    [GeneratedRegex(@"^(\d);(\d)$")]
    private static partial Regex SpecificationRegex();
}
