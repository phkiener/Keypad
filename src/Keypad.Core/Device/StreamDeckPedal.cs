using System.Globalization;
using HidSharp;
using Keypad.Core.Abstractions;

namespace Keypad.Core.Device;

/// <summary>
/// A <see cref="ConnectedDevice"/> for the <see cref="DeviceType.StreamDeckXL2022"/>
/// </summary>
public sealed class StreamDeckPedal : ConnectedDevice
{
    /// <inheritdoc />
    protected override int Rows => 1;
    
    /// <inheritdoc />
    protected override int Columns => 3;

    /// <inheritdoc />
    protected override Dictionary<byte, DeviceButton> ButtonIndex { get; } = new();
    
    /// <inheritdoc />
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

    /// <inheritdoc />
    public override bool SetSensitivity(double sensitivity)
    {
        var value = Math.Clamp(sensitivity, 0, .7) * 100;

        var request = new byte[32];
        request[0] = 0x02;
        request[1] = 0x0A;
        request[2] = byte.CreateTruncating(value);
        request[3] = 0;
        
        using var stream = Device.Open();
        stream.SetFeature(request);

        return true;
    }

    /// <inheritdoc />
    public override bool SetStatusLED(string color)
    {
        var red = color.StartsWith('#') ? color[1..3] : color[0..2];
        var green = color.StartsWith('#') ? color[3..5] : color[2..4];
        var blue = color.StartsWith('#') ? color[5..6] : color[4..5];

        var message = new byte[1024];
        message[0] = 0x02;
        message[1] = 0x0B;
        message[2] = byte.Parse(red, NumberStyles.HexNumber);
        message[3] = byte.Parse(green, NumberStyles.HexNumber);
        message[4] = byte.Parse(blue, NumberStyles.HexNumber);
        message[5] = 0x02;
        
        using var stream = Device.Open();
        stream.Write(message);

        return true;
    }
}
