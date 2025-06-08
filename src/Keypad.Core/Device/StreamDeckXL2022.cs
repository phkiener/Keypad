using HidSharp;
using Keypad.Core.Abstractions;
using Keypad.Core.Image;
using Keypad.Core.Util;
using SkiaSharp;

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

    /// <inheritdoc />
    public override IEnumerable<DeviceButton> Buttons => buttonIndex.Values;

    /// <inheritdoc />
    public override bool SetBrightness(double brightness)
    {
        var value = Math.Clamp(brightness, 0, 1) * 100;

        var brightnessRequest = new byte[32];
        brightnessRequest[0] = 0x03;
        brightnessRequest[1] = 0x08;
        brightnessRequest[2] = byte.CreateTruncating(value);
        
        using var stream = Device.Open();
        stream.SetFeature(brightnessRequest);

        return true;
    }
    
    /// <inheritdoc />
    public override bool SetImage(DeviceButton button, DeviceImage image)
    {
        var buttonId = buttonIndex.SingleOrDefault(b => b.Value == button).Key;
        var imageData = ConvertButtonImage(image);

        const int setImageLength = 1024;
        const int headerLength = 8;
        
        short counter = 0;
        var index = 0;
        
        using var stream = Device.Open();
        while (index < imageData.Length)
        {
            var message = new byte[setImageLength];
            using var messageStream = new MemoryStream(message);
            
            var currentChunk = imageData.Slice(index, Math.Min(imageData.Length - index, setImageLength - headerLength));
            short chunkSize = (short)currentChunk.Length;
            
            messageStream.WriteByte(0x02);
            messageStream.WriteByte(0x07);
            messageStream.WriteByte(buttonId);
            messageStream.WriteByte(index + chunkSize + 1 >= imageData.Length ? (byte)0x01 : (byte)0x00);

            messageStream.Write(chunkSize.ToLittleEndian());
            messageStream.Write(counter.ToLittleEndian());
            messageStream.Write(currentChunk);
            
            stream.Write(message);

            counter += 1;
            index += currentChunk.Length;
        }

        return true;
    }

    /// <inheritdoc />
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

    private static ReadOnlySpan<byte> ConvertButtonImage(DeviceImage image)
    {
        const int imageWidth = 96;
        const int imageHeight = 96;

        using var bitmap = ImageLoader.Load(image, imageWidth, imageHeight);
        using var flippedBitmap = bitmap.Flip(horizontal: false, vertical: true);
        
        return flippedBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
    }
}
