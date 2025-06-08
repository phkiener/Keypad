namespace Keypad.Core.Abstractions;

/// <summary>
/// An object to emit key presses to the operating system
/// </summary>
public interface IKeyEmitter
{
    /// <summary>
    /// Emit a pressing (i.e. holding down) of the given key
    /// </summary>
    /// <param name="key">The <see cref="EmulatedKey"/> to press</param>
    void Press(EmulatedKey key);
    
    /// <summary>
    /// Emit a releasing (i.e. letting go) of the given key
    /// </summary>
    /// <param name="key">The <see cref="EmulatedKey"/> to release</param>
    void Release(EmulatedKey key);
}
