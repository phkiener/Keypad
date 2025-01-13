using System.Runtime.InteropServices;

namespace PlainDeck.Sdk;

public static partial class KeyEmitter
{
    public static void KeyDown(ConsoleKey key)
    {
        if (OperatingSystem.IsWindows())
        {
            var keyInfo = MapKeyWindows(key);
            SendKeyWindows(keyInfo, 0, 0x0001, 0);

            return;
        }

        throw new NotSupportedException($"{nameof(KeyEmitter)} is not implemented for {Environment.OSVersion.Platform}.");
    }

    public static void KeyUp(ConsoleKey key)
    {
        if (OperatingSystem.IsWindows())
        {
            var keyInfo = MapKeyWindows(key);
            SendKeyWindows(keyInfo, 0, 0x0002 | 0x0001, 0);

            return;
        }

        throw new NotSupportedException($"{nameof(KeyEmitter)} is not implemented for {Environment.OSVersion.Platform}.");
    }
    
    [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
    private static partial void SendKeyWindows(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
    
    // https://learn.microsoft.com/de-de/windows/win32/inputdev/virtual-key-codes
    private static byte MapKeyWindows(ConsoleKey key)
    {
        return (byte)key;
    }

}
