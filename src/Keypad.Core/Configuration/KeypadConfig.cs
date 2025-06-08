using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core.Abstractions;

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
    /// The sensitivity to set for the keys; defaults to 0.5
    /// </summary>
    [JsonPropertyName("sensitivity")]
    public double Sensitivity { get; set; } = 0.5;
    
    /// <summary>
    /// The color to set for the status LED; defaults to #FFFFFF
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = "#FFFFFF";
    
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
    [JsonConverter(typeof(KeypadKeyConverter))]
    public required EmulatedKey Key { get; set; }
    
    /// <summary>
    /// Image to set for the button on the device
    /// </summary>
    [JsonPropertyName("image")]
    [JsonConverter(typeof(DeviceImageConverter))]
    public DeviceImage? Image { get; set; }
}
