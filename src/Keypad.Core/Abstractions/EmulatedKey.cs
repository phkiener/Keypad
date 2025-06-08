namespace Keypad.Core.Abstractions;

/// <summary>
/// An emulated keypress on the host
/// </summary>
/// <param name="Code">The <see cref="Keycode"/> that is pressed</param>
public readonly record struct EmulatedKey(EmulatedKey.Keycode Code)
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
        A     = 0x01, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Key0  = 0x20, Key1, Key2, Key3, Key4, Key5, Key6, Key7, Key8, Key9,
        Num0  = 0x30, Num1, Num2, Num3, Num4, Num5, Num6, Num7, Num8, Num9,
        F1    = 0x41, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20
    }
}
