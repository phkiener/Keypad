using HidSharp;
using Keypad.Core.Abstractions;

namespace Keypad.Core.Device;

/// <summary>
/// A <see cref="ConnectedDevice"/> for the <see cref="DeviceType.StreamDeckXL2022"/>
/// </summary>
public sealed class StreamDeckXL2022 : ConnectedDevice
{
    private static readonly Dictionary<byte, DeviceButton> buttonIndex = new();
    private const int Rows = 4;
    private const int Columns = 8;
    
    private readonly Dictionary<DeviceButton, ButtonState> keymap;

    static StreamDeckXL2022()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                var button = new DeviceButton(row + 1, column + 1);
                var buttonId = row * Columns + column;
                
                buttonIndex.Add(byte.CreateChecked(buttonId), button);
            }
        }
    }
    
    internal StreamDeckXL2022(HidDevice device) : base(device)
    {
        keymap = buttonIndex.Values.ToDictionary(static key => key, static _ => ButtonState.Up);
    }

    protected override void OnMessageReceived(ReadOnlySpan<byte> payload)
    {
        if (payload is [0x01, 0x00, 0x20, 0x00, ..] && payload.Length >= 4 + Rows * Columns)
        {
            for (byte i = 0; i < Rows * Columns; i++)
            {
                var buttonState = (ButtonState)payload[4 + i];
                var deviceButton = buttonIndex[i];
                
                var currentState = keymap[deviceButton];
                if (currentState == buttonState)
                {
                    continue;
                }

                keymap[deviceButton] = buttonState;
                if (buttonState is ButtonState.Down)
                {
                    OnKeyPressed(deviceButton);
                }

                if (buttonState is ButtonState.Up)
                {
                    OnKeyReleased(deviceButton);
                }
            }
        }
    }
}
