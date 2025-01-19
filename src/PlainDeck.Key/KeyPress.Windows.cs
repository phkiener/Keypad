using System.Diagnostics;
using PlainDeck.Extensions.NativeBindings;

namespace PlainDeck.Extensions;

public static partial class KeyPress
{
    private static void KeyDown_Windows(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsWindows());

        var keyCode = KeyCode.Map(key);
        Windows.KeybdEvent(keyCode, 0, Windows.KeyEventF_ExtendedKey, IntPtr.Zero);
    }
    
    private static void KeyUp_Windows(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsWindows());

        var keyCode = KeyCode.Map(key);
        Windows.KeybdEvent(keyCode, 0, Windows.KeyEventF_KeyUp | Windows.KeyEventF_ExtendedKey, IntPtr.Zero);
    }
}

file static class KeyCode
{
    public static byte Map(ConsoleKey key)
    {
        return (byte)key; // seems to match
    }
}
