using System.Runtime.InteropServices;

namespace PlainDeck.Extensions.NativeBindings;

internal static partial class MacOS
{
    public const ushort NX_KEYTYPE_NEXT = 17;
    public const int kCGEventSourceStateHIDSystemState = 1;
    public const uint KeyDown = 10 << 8;
    public const uint KeyUp = 11 << 8;
    
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
