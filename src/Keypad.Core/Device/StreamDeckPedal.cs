using HidSharp;
using Keypad.Core.Abstractions;

namespace Keypad.Core.Device;

/// <summary>
/// A <see cref="ConnectedDevice"/> for the <see cref="DeviceType.StreamDeckXL2022"/>
/// </summary>
public sealed class StreamDeckPedal : ConnectedDevice
{
    protected override int Rows => 1;
    protected override int Columns => 3;

    protected override Dictionary<byte, DeviceButton> ButtonIndex { get; } = new();
    protected override Dictionary<DeviceButton, ButtonState> Keymap { get; }

    internal StreamDeckPedal(HidDevice device) : base(device)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                var button = new DeviceButton(row + 1, column + 1);
                var buttonId = row * Columns + column;
                
                ButtonIndex.Add(byte.CreateChecked(buttonId), button);
            }
        }
        
        Keymap = ButtonIndex.Values.ToDictionary(static key => key, static _ => ButtonState.Up);
    }
}
