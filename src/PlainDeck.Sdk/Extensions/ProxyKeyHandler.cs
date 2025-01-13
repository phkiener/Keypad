using System.Runtime.InteropServices;

namespace PlainDeck.Sdk.Extensions;

public partial class ProxyKeyHandler(ConsoleKey key) : KeyHandler
{
    public override Task OnKeyDown(DeviceContext context)
    {
        if (OperatingSystem.IsWindows())
        {
            SendKeyWindows(MapKeyWindows(key), 0, 0x0001, 0);
            return Task.CompletedTask;
        }

        if (OperatingSystem.IsMacOS())
        {
            var e = CGEventCreateKeyboardEvent(0x00, MapKeyMacOS(key), true);
            CGEventPost(0, e);
            CFRelease(e);

            return Task.CompletedTask;
        }

        throw new NotSupportedException($"Platform {Environment.OSVersion} is not supported");
    }

    public override Task OnKeyUp(DeviceContext context)
    {
        if (OperatingSystem.IsWindows())
        {
            SendKeyWindows(MapKeyWindows(key), 0, 0x0002 | 0x0001, 0);
            return Task.CompletedTask;
        }

        if (OperatingSystem.IsMacOS())
        {
            
            var e = CGEventCreateKeyboardEvent(0x00, MapKeyMacOS(key), false);
            CGEventPost(0, e);
            CFRelease(e);
            return Task.CompletedTask;
        }

        throw new NotSupportedException($"Platform {Environment.OSVersion} is not supported");
    }
    
    [LibraryImport("user32.dll", EntryPoint = "keybd_event")]
    private static partial void SendKeyWindows(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    private static byte MapKeyWindows(ConsoleKey key)
    {
        return (byte)key;
    }
    
    private static ushort MapKeyMacOS(ConsoleKey key)
    {
        return key switch
        {
            ConsoleKey.A => 0x00,
            ConsoleKey.S => 0x01,
            ConsoleKey.D => 0x02,
            ConsoleKey.F => 0x03,
            ConsoleKey.H => 0x04,
            ConsoleKey.G => 0x05,
            ConsoleKey.Z => 0x06,
            ConsoleKey.X => 0x07,
            ConsoleKey.C => 0x08,
            ConsoleKey.V => 0x09,
            ConsoleKey.B => 0x0B,
            ConsoleKey.Q => 0x0C,
            ConsoleKey.W => 0x0D,
            ConsoleKey.E => 0x0E,
            ConsoleKey.R => 0x0F,
            ConsoleKey.Y => 0x10,
            ConsoleKey.T => 0x11,
            ConsoleKey.D1 => 0x12,
            ConsoleKey.D2 => 0x13,
            ConsoleKey.D3 => 0x14,
            ConsoleKey.D4 => 0x15,
            ConsoleKey.D6 => 0x16,
            ConsoleKey.D5 => 0x17,
            ConsoleKey.D9 => 0x19,
            ConsoleKey.D7 => 0x1A,
            ConsoleKey.OemMinus => 0x1B,
            ConsoleKey.D8 => 0x1C,
            ConsoleKey.D0 => 0x1D,
            ConsoleKey.O => 0x1F,
            ConsoleKey.U => 0x20,
            ConsoleKey.I => 0x22,
            ConsoleKey.P => 0x23,
            ConsoleKey.L => 0x25,
            ConsoleKey.J => 0x26,
            ConsoleKey.K => 0x28,
            ConsoleKey.OemComma => 0x2B,
            ConsoleKey.N => 0x2D,
            ConsoleKey.M => 0x2E,
            ConsoleKey.OemPeriod => 0x2F,
            ConsoleKey.Multiply => 0x43,
            ConsoleKey.OemPlus => 0x45,
            ConsoleKey.OemClear => 0x47,
            ConsoleKey.Divide => 0x4B,
            ConsoleKey.Enter => 0x4C,
            ConsoleKey.NumPad0 => 0x52,
            ConsoleKey.NumPad1 => 0x53,
            ConsoleKey.NumPad2 => 0x54,
            ConsoleKey.NumPad3 => 0x55,
            ConsoleKey.NumPad4 => 0x56,
            ConsoleKey.NumPad5 => 0x57,
            ConsoleKey.NumPad6 => 0x58,
            ConsoleKey.NumPad7 => 0x59,
            ConsoleKey.NumPad8 => 0x5B,
            ConsoleKey.NumPad9 => 0x5C,
            _ => throw new NotSupportedException($"Key {key} not supported")
        };
    }

    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGEventCreateKeyboardEvent(IntPtr source, ushort key, [MarshalAs(UnmanagedType.Bool)] bool keyDown);
    
    [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static partial IntPtr CGEventPost(ushort tap, IntPtr eventRef);
    
    [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
    private static partial void CFRelease(IntPtr typeRef);
}
