using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PlainDeck.Sdk.Extensions;

public sealed partial class ProxyKeyHandler
{
    private static Task KeyDown_MacOS(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsMacOS());
        
        // what the fuck
        if (key is ConsoleKey.MediaNext)
        {
            var source = MacOS.CGEventSourceCreate(MacOS.kCGEventSourceStateHIDSystemState);
            var eventRef = MacOS.CGEventCreate(source);
            
            MacOS.CGEventSetType(eventRef, 14);
            MacOS.CGEventSetFlags(eventRef, MacOS.KeyDown);
            MacOS.CGEventSetIntegerValueField(eventRef, 0x53, 8);
            MacOS.CGEventSetIntegerValueField(eventRef, 0x95, MacOS.KeyDown | (MacOS.NX_KEYTYPE_NEXT << 16));
            MacOS.CGEventSetIntegerValueField(eventRef, 0x96, -1);

            MacOS.CGEventPost(0, eventRef);
            MacOS.CFRelease(eventRef);

            return Task.CompletedTask;
        }

        var keyCode = MacOS.MapKey(key);
        var keyboardEvent = MacOS.CGEventCreateKeyboardEvent(source: IntPtr.Zero, virtualKey: keyCode, keyDown: true);

        MacOS.CGEventPost(0, keyboardEvent);
        MacOS.CFRelease(keyboardEvent);

        return Task.CompletedTask;
    }
    
    private static Task KeyUp_MacOS(ConsoleKey key)
    {
        Debug.Assert(OperatingSystem.IsMacOS());

        if (key is ConsoleKey.MediaNext)
        {
            var source = MacOS.CGEventSourceCreate(MacOS.kCGEventSourceStateHIDSystemState);
            var eventRef = MacOS.CGEventCreate(source);
            
            MacOS.CGEventSetType(eventRef, 14);
            MacOS.CGEventSetFlags(eventRef, MacOS.KeyUp);
            MacOS.CGEventSetIntegerValueField(eventRef, 0x53, 8);
            MacOS.CGEventSetIntegerValueField(eventRef, 0x95, MacOS.KeyUp | (MacOS.NX_KEYTYPE_NEXT << 16));
            MacOS.CGEventSetIntegerValueField(eventRef, 0x96, -1);

            MacOS.CGEventPost(0, eventRef);
            MacOS.CFRelease(eventRef);

            return Task.CompletedTask;
        }

        var keyCode = MacOS.MapKey(key);
        var keyboardEvent = MacOS.CGEventCreateKeyboardEvent(source: IntPtr.Zero, virtualKey: keyCode, keyDown: false);

        MacOS.CGEventPost(0, keyboardEvent);
        MacOS.CFRelease(keyboardEvent);
        
        return Task.CompletedTask;
    }

    private static partial class MacOS
    {
        public const ushort NX_KEYTYPE_NEXT = 17;
        public const int kCGEventSourceStateHIDSystemState = 1;
        public const uint KeyDown = 10 << 8;
        public const uint KeyUp = 11 << 8;

        public static ushort MapKey(ConsoleKey key)
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
        public static partial IntPtr CGEventSourceCreate(int stateID);
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial IntPtr CGEventCreate(IntPtr source);
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial void CGEventSetType(IntPtr @event, uint eventType);
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial void CGEventSetFlags(IntPtr @event, ulong flags);
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial void CGEventSetIntegerValueField(IntPtr @event, uint feld, long value);
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial IntPtr CGEventCreateKeyboardEvent(IntPtr source, ushort virtualKey, [MarshalAs(UnmanagedType.Bool)] bool keyDown);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static partial void CGEventPost(uint tap, IntPtr @event);
    
        [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        public static partial void CFRelease(IntPtr cf);
    }
}
