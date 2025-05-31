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
public readonly record struct KeypadKey(KeypadKey.Keycode Key)
{
    public bool Control { get; init; }
    public bool Shift { get; init; }
    public bool Option { get; init; }
    public bool Command { get; init; }
    
    public enum Keycode
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }
}

[JsonConverter(typeof(KeypadButtonConverter))]
public readonly record struct KeypadButton(int Row, int Column);

[JsonConverter(typeof(KeypadImageConverter))]
public interface KeypadImage
{
    public sealed record Color(string Value) : KeypadImage;
    public sealed record File(string Path) : KeypadImage;
}
