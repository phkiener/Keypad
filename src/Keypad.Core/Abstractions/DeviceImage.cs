namespace Keypad.Core.Abstractions;

/// <summary>
/// Image to set on a device button
/// </summary>
public interface DeviceImage
{
    /// <summary>
    /// Solid color image of a given color
    /// </summary>
    /// <param name="Value">The color to set; supported are all valid CSS colors</param>
    public sealed record Color(string Value) : DeviceImage;
    
    /// <summary>
    /// Image that is loaded from a file
    /// </summary>
    /// <param name="Path">Path to the file to load</param>
    public sealed record File(string Path) : DeviceImage;
}
