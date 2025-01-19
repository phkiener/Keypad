using System.Runtime.InteropServices;

namespace PlainDeck.Extensions.NativeBindings;

internal static partial class Windows
{
    public const uint KeyEventF_ExtendedKey = 0x0001;
    public const uint KeyEventF_KeyUp = 0x0002;
        
    [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
    public static partial void KeybdEvent(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
}
