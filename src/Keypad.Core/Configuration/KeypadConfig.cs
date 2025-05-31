using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;

namespace Keypad.Configuration;

public sealed class KeypadConfig
{
    [JsonRequired]
    [JsonPropertyName("devices")]
    public required KeypadDeviceConfiguration[] Devices { get; set; }
}

public sealed class KeypadDeviceConfiguration
{
    [JsonRequired]
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("serial")]
    public string? SerialNumber { get; set; } = null;

    [JsonPropertyName("brightness")]
    public double Brightness { get; set; } = 0.7;
    
    [JsonRequired]
    [JsonPropertyName("keys")]
    public required KeypadKeyConfiguration[] Keys { get; set; }
}

public sealed class KeypadKeyConfiguration
{
    [JsonRequired]
    [JsonPropertyName("key")]
    public required KeypadKey Key { get; set; }

    [JsonRequired]
    [JsonPropertyName("button")]
    public required KeypadButton Button { get; set; }
    
    [JsonRequired]
    [JsonPropertyName("image")]
    public required KeypadImage Image { get; set; }
}

[JsonConverter(typeof(KeypadKeyConverter))]
public readonly record struct KeypadKey();

[JsonConverter(typeof(KeypadButtonConverter))]
public readonly record struct KeypadButton(int Row, int Column);

[JsonConverter(typeof(KeypadImageConverter))]
public interface KeypadImage
{
    public sealed class Color : KeypadImage
    {
        public required string Value { get; set; }
    }
    
    public sealed class Text : KeypadImage
    {
        public required string Content { get; set; }
        public required string Foreground { get; set; }
        public required string Background { get; set; }
    }
    
    public sealed class File : KeypadImage
    {
        public required string Path { get; set; }
    }
    
    public sealed class Inline : KeypadImage
    {
        public required string SvgContent { get; set; }
    }
}
