using Keypad.Core.Abstractions;

namespace Keypad.MacOS;

public sealed class SendKey : IKeyEmitter
{
    public void Press(EmulatedKey key)
    {
        using var eventSource = new CGEventSource(CGEventSourceStateID.HidSystem);
        using var keyEvent = new CGEvent(eventSource, key.GetKeyCode(), keyDown: true).SetFlags(key);
        CGEvent.Post(keyEvent, CGEventTapLocation.HID);
    }

    public void Release(EmulatedKey key)
    {
        using var eventSource = new CGEventSource(CGEventSourceStateID.HidSystem);
        using var keyEvent = new CGEvent(eventSource, key.GetKeyCode(), keyDown: false).SetFlags(key);
        CGEvent.Post(keyEvent, CGEventTapLocation.HID);
    }
}

file static class EventExtensions
{
    public static ushort GetKeyCode(this EmulatedKey key)
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
            EmulatedKey.Keycode.Key0 => NSKey.D0,
            EmulatedKey.Keycode.Key1 => NSKey.D1,
            EmulatedKey.Keycode.Key2 => NSKey.D2,
            EmulatedKey.Keycode.Key3 => NSKey.D3,
            EmulatedKey.Keycode.Key4 => NSKey.D4,
            EmulatedKey.Keycode.Key5 => NSKey.D5,
            EmulatedKey.Keycode.Key6 => NSKey.D6,
            EmulatedKey.Keycode.Key7 => NSKey.D7,
            EmulatedKey.Keycode.Key8 => NSKey.D8,
            EmulatedKey.Keycode.Key9 => NSKey.D9,
            EmulatedKey.Keycode.Num0 => NSKey.Keypad0,
            EmulatedKey.Keycode.Num1 => NSKey.Keypad1,
            EmulatedKey.Keycode.Num2 => NSKey.Keypad2,
            EmulatedKey.Keycode.Num3 => NSKey.Keypad3,
            EmulatedKey.Keycode.Num4 => NSKey.Keypad4,
            EmulatedKey.Keycode.Num5 => NSKey.Keypad5,
            EmulatedKey.Keycode.Num6 => NSKey.Keypad6,
            EmulatedKey.Keycode.Num7 => NSKey.Keypad7,
            EmulatedKey.Keycode.Num8 => NSKey.Keypad8,
            EmulatedKey.Keycode.Num9 => NSKey.Keypad9,
            EmulatedKey.Keycode.F1 => NSKey.F1,
            EmulatedKey.Keycode.F2 => NSKey.F2,
            EmulatedKey.Keycode.F3 => NSKey.F3,
            EmulatedKey.Keycode.F4 => NSKey.F4,
            EmulatedKey.Keycode.F5 => NSKey.F5,
            EmulatedKey.Keycode.F6 => NSKey.F6,
            EmulatedKey.Keycode.F7 => NSKey.F7,
            EmulatedKey.Keycode.F8 => NSKey.F8,
            EmulatedKey.Keycode.F9 => NSKey.F9,
            EmulatedKey.Keycode.F10 => NSKey.F10,
            EmulatedKey.Keycode.F11 => NSKey.F11,
            EmulatedKey.Keycode.F12 => NSKey.F12,
            EmulatedKey.Keycode.F13 => NSKey.F13,
            EmulatedKey.Keycode.F14 => NSKey.F14,
            EmulatedKey.Keycode.F15 => NSKey.F15,
            EmulatedKey.Keycode.F16 => NSKey.F16,
            EmulatedKey.Keycode.F17 => NSKey.F17,
            EmulatedKey.Keycode.F18 => NSKey.F18,
            EmulatedKey.Keycode.F19 => NSKey.F19,
            EmulatedKey.Keycode.F20 => NSKey.F20,
            _ => throw new ArgumentOutOfRangeException()
        };

        return (ushort)platformKey;
    }

    public static CGEvent SetFlags(this CGEvent keyEvent, EmulatedKey key)
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
