namespace Keypad.Core.Abstractions;

/// <summary>
/// An emulated keypress on the host
/// </summary>
/// <param name="Key">The <see cref="Keycode"/> that is pressed</param>
public readonly record struct EmulatedKey(EmulatedKey.Keycode Key)
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
