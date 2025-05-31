using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core;
using Keypad.Core.Device;

namespace Keypad.Configuration;

/// <summary>
/// Configuration for keypad
/// </summary>
public sealed class KeypadConfig
{
    /// <summary>
    /// List of devices that are configured
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("devices")]
    public required KeypadDeviceConfiguration[] Devices { get; set; }
}

/// <summary>
/// Configuration for a single device
/// </summary>
public sealed class KeypadDeviceConfiguration
{
    /// <summary>
    /// Type of device to connect to
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("type")]
    [JsonConverter(typeof(DeviceTypeConverter))]
    public required DeviceType Type { get; set; }
    
    /// <summary>
    /// Optional serial number of the device to connect to
    /// </summary>
    /// <remarks>
    /// Use this if you've got multiple devices of the same type.
    /// </remarks>
    [JsonPropertyName("serial")]
    public string? SerialNumber { get; set; } = null;

    /// <summary>
    /// The brightness to set for the keys; defaults to 0.7
    /// </summary>
    [JsonPropertyName("brightness")]
    public double Brightness { get; set; } = 0.7;
    
    /// <summary>
    /// Configurations for the keys on the device
    /// </summary>
    /// <remarks>
    /// Keys that are not configured will be set to a solid black image and otherwise ignored
    /// </remarks>
    [JsonRequired]
    [JsonPropertyName("keys")]
    public required KeypadKeyConfiguration[] Keys { get; set; }
}

/// <summary>
/// Configuration for a single key
/// </summary>
public sealed class KeypadKeyConfiguration
{
    /// <summary>
    /// Button on the device to configure
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("button")]
    [JsonConverter(typeof(DeviceButtonConverter))]
    public required DeviceButton Button { get; set; }
    
    /// <summary>
    /// Key to emulate on the host
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("key")]
    public required KeypadKey Key { get; set; }
    
    /// <summary>
    /// Image to set for the button on the device
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("image")]
    public required KeypadImage Image { get; set; }
}

/// <summary>
/// An emulated keypress on the host
/// </summary>
/// <param name="Key">The <see cref="Keycode"/> that is pressed</param>
[JsonConverter(typeof(KeypadKeyConverter))]
public readonly record struct KeypadKey(KeypadKey.Keycode Key)
{
    /// <summary>
    /// Whether the Control modifier should be emitted
    /// </summary>
    public bool Control { get; init; }
    
    /// <summary>
    /// Whether the Shift modifier should be emitted
    /// </summary>
    public bool Shift { get; init; }
    
    /// <summary>
    /// Whether the Option modifier should be emitted
    /// </summary>
    public bool Option { get; init; }
    
    /// <summary>
    /// Whether the Command modifier should be emitted
    /// </summary>
    public bool Command { get; init; }
    
    /// <summary>
    /// A keycode to send
    /// </summary>
    public enum Keycode
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
    }
}

/// <summary>
/// Image to set on a device button
/// </summary>
[JsonConverter(typeof(KeypadImageConverter))]
public interface KeypadImage
{
    /// <summary>
    /// Solid color image of a given color
    /// </summary>
    /// <param name="Value">The color to set; supported are all valid CSS colors</param>
    public sealed record Color(string Value) : KeypadImage;
    
    /// <summary>
    /// Image that is loaded from a file
    /// </summary>
    /// <param name="Path">Path to the file to load</param>
    public sealed record File(string Path) : KeypadImage;
}
