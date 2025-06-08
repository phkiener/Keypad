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
    /// <inheritdoc />
    protected override int Rows => 4;
    
    /// <inheritdoc />
    protected override int Columns => 8;

    /// <inheritdoc />
    protected override Dictionary<byte, DeviceButton> ButtonIndex { get; } = new();
    
    /// <inheritdoc />
    protected override Dictionary<DeviceButton, ButtonState> Keymap { get; }
    
    internal StreamDeckXL2022(HidDevice device) : base(device)
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
    public override bool Sleep()
    {
        var request = new byte[32];
        request[0] = 0x03;
        request[1] = 0x02;

        using var stream = Device.Open();
        stream.SetFeature(request);

        return true;
    }

    /// <inheritdoc />
    public override bool Wake()
    {
        var request = new byte[32];
        request[0] = 0x03;
        request[1] = 0x05;

        using var stream = Device.Open();
        stream.SetFeature(request);

        return true;
    }

    /// <inheritdoc />
    public override bool SetBrightness(double brightness)
    {
        var value = Math.Clamp(brightness, 0, 1) * 100;

        var request = new byte[32];
        request[0] = 0x03;
        request[1] = 0x08;
        request[2] = byte.CreateTruncating(value);
        
        using var stream = Device.Open();
        stream.SetFeature(request);

        return true;
    }
    
    /// <inheritdoc />
    public override bool SetImage(DeviceButton button, DeviceImage image)
    {
        var buttonId = ButtonIndex.SingleOrDefault(b => b.Value == button).Key;
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
    public override bool SetImage(DeviceImage image)
    {
        var imageData = ConvertScreenImage(image);
        
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
            messageStream.WriteByte(0x08);
            messageStream.WriteByte(0x00);
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
    public override bool SetScreensaver(DeviceImage image, TimeSpan delay)
    {
        SetTimeout(delay);
        SetTimeoutImage(image);
        
        return true;
    }

    private void SetTimeout(TimeSpan delay)
    {
        var request = new byte[32];
        request[0] = 0x03;
        request[1] = 0x08;
        
        var timeoutSeconds = ((short)delay.TotalSeconds).ToLittleEndian();
        request[2] = timeoutSeconds[0];
        request[3] = timeoutSeconds[1];
        
        using var stream = Device.Open();
        stream.SetFeature(request);
    }

    private void SetTimeoutImage(DeviceImage image)
    {
        var imageData = ConvertScreenImage(image);
        
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
            messageStream.WriteByte(0x09);
            messageStream.WriteByte(0x08);
            messageStream.WriteByte(index + chunkSize + 1 >= imageData.Length ? (byte)0x01 : (byte)0x00);

            messageStream.Write(chunkSize.ToLittleEndian());
            messageStream.Write(counter.ToLittleEndian());
            messageStream.Write(currentChunk);
            
            stream.Write(message);

            counter += 1;
            index += currentChunk.Length;
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

    private static ReadOnlySpan<byte> ConvertScreenImage(DeviceImage image)
    {
        const int imageWidth = 1024;
        const int imageHeight = 600;

        using var bitmap = ImageLoader.Load(image, imageWidth, imageHeight);
        using var flippedBitmap = bitmap.Flip(horizontal: true, vertical: true);
        
        return flippedBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
    }
}
