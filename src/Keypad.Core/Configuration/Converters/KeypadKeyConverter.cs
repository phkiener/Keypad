using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Keypad.Configuration.Converters;

internal sealed partial class KeypadKeyConverter : JsonConverter<KeypadKey>
{
    public override KeypadKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var specification = reader.GetString();

        var match = SpecificationRegex().Match(specification ?? string.Empty);
        if (!match.Success)
        {
            throw new FormatException($"Invalid key specification '{specification}'.");
        }

        var key = ParseKey(match.Groups[3].Value);
        foreach (var modifier in match.Groups[2].Captures)
        {
            key = modifier.ToString() switch
            {
                "Shift" => key with { Shift = true },
                "Control" or "Ctrl" => key with { Control = true },
                "Command" or "Cmd" => key with { Command = true },
                "Option" or "Opt" => key with { Option = true },
                _ => throw new FormatException($"Invalid modifier '{modifier}'.")
            };
        }

        return key;
    }

    public override void Write(Utf8JsonWriter writer, KeypadKey value, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter is read-only.");
    }

    private static KeypadKey ParseKey(string code)
    {
        if (code is [>= 'A' and <= 'Z'])
        {
            var keyCode = KeypadKey.Keycode.A + (code[0] - 'A');
            return new KeypadKey(keyCode);
        }

        throw new FormatException($"Unknown keycode '{code}'.");
    }

    [GeneratedRegex(@"^(([a-zA-Z]+)\+)*([a-zA-Z0-9]+)$")]
    private static partial Regex SpecificationRegex();
}
