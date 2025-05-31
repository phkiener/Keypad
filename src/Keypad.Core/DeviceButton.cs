namespace Keypad.Core;

/// <summary>
/// Describes a button on the device
/// </summary>
/// <param name="Row">1-based index of the row this button is in</param>
/// <param name="Column">1-based index of the column this button is in</param>
public readonly record struct DeviceButton(int Row, int Column);

/// <summary>
/// The state of a button
/// </summary>
public enum ButtonState : byte
{
    /// <summary>
    /// The button is currently being held
    /// </summary>
    Down = 1,
    
    /// <summary>
    /// The button is not pressed
    /// </summary>
    Up = 0
}
