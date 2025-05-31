using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Keypad.Configuration.Converters;

public sealed partial class KeypadButtonConverter : JsonConverter<KeypadButton>
{
    public override KeypadButton Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var specification = reader.GetString();

        var match = SpecificationRegex().Match(specification ?? string.Empty);
        if (!match.Success)
        {
            throw new FormatException($"Invalid button specification '{specification}'.");
        }
        
        var row = int.Parse(match.Groups[1].Value);
        var column = int.Parse(match.Groups[2].Value);

        return new KeypadButton(row, column);
    }

    public override void Write(Utf8JsonWriter writer, KeypadButton value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }

    [GeneratedRegex(@"^(\d);(\d)$")]
    private static partial Regex SpecificationRegex();
}
