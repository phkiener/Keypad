using Keypad.Core.Abstractions;

namespace Keypad;

public static class SendKey
{
    public static void KeyDown(EmulatedKey key)
    {
        using var eventSource = new CGEventSource(CGEventSourceStateID.HidSystem);
        using var keyEvent = new CGEvent(eventSource, key.GetKeyCode(), keyDown: true).SetFlags(key);
        CGEvent.Post(keyEvent, CGEventTapLocation.HID);
    }

    public static void KeyUp(EmulatedKey key)
    {
        using var eventSource = new CGEventSource(CGEventSourceStateID.HidSystem);
        using var keyEvent = new CGEvent(eventSource, key.GetKeyCode(), keyDown: false).SetFlags(key);
        CGEvent.Post(keyEvent, CGEventTapLocation.HID);
    }

    private static ushort GetKeyCode(this EmulatedKey key)
    {
        var platformKey = key.Code switch
        {
            EmulatedKey.Keycode.A => NSKey.A,
            EmulatedKey.Keycode.B => NSKey.B,
            EmulatedKey.Keycode.C => NSKey.C,
            EmulatedKey.Keycode.D => NSKey.D,
            EmulatedKey.Keycode.E => NSKey.E,
            EmulatedKey.Keycode.F => NSKey.F,
            EmulatedKey.Keycode.G => NSKey.G,
            EmulatedKey.Keycode.H => NSKey.H,
            EmulatedKey.Keycode.I => NSKey.I,
            EmulatedKey.Keycode.J => NSKey.J,
            EmulatedKey.Keycode.K => NSKey.K,
            EmulatedKey.Keycode.L => NSKey.L,
            EmulatedKey.Keycode.M => NSKey.M,
            EmulatedKey.Keycode.N => NSKey.N,
            EmulatedKey.Keycode.O => NSKey.O,
            EmulatedKey.Keycode.P => NSKey.P,
            EmulatedKey.Keycode.Q => NSKey.Q,
            EmulatedKey.Keycode.R => NSKey.R,
            EmulatedKey.Keycode.S => NSKey.S,
            EmulatedKey.Keycode.T => NSKey.T,
            EmulatedKey.Keycode.U => NSKey.U,
            EmulatedKey.Keycode.V => NSKey.V,
            EmulatedKey.Keycode.W => NSKey.W,
            EmulatedKey.Keycode.X => NSKey.X,
            EmulatedKey.Keycode.Y => NSKey.Y,
            EmulatedKey.Keycode.Z => NSKey.Z,
            _ => throw new ArgumentOutOfRangeException()
        };

        return (ushort)platformKey;
    }

    private static CGEvent SetFlags(this CGEvent keyEvent, EmulatedKey key)
    {
        if (key.Shift)
        {
            keyEvent.Flags |= CGEventFlags.Shift;
        }
        
        if (key.Control)
        {
            keyEvent.Flags |= CGEventFlags.Control;
        }
        
        if (key.Option)
        {
            keyEvent.Flags |= CGEventFlags.Alternate;
        }
        
        if (key.Command)
        {
            keyEvent.Flags |= CGEventFlags.Command;
        }

        return keyEvent;
    }
}
