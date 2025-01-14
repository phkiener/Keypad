using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PlainDeck.Extensions;

public sealed partial class ProxyKeyHandler
{
    private static Task KeyDown_Windows(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsWindows());

        var keyCode = (byte)key;
        Windows.KeybdEvent(keyCode, 0, Windows.KeyEventF_ExtendedKey, IntPtr.Zero);
        
        return Task.CompletedTask;
    }
    
    private static Task KeyUp_Windows(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsWindows());

        var keyCode = (byte)key;
        Windows.KeybdEvent(keyCode, 0, Windows.KeyEventF_KeyUp | Windows.KeyEventF_ExtendedKey, IntPtr.Zero);
        
        return Task.CompletedTask;
    }

    private static partial class Windows
    {
        public const uint KeyEventF_ExtendedKey = 0x0001;
        public const uint KeyEventF_KeyUp = 0x0002;
        
        [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
        public static partial void KeybdEvent(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
    }
}
