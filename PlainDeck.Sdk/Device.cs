using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using HidSharp;
using PlainDeck.Sdk.Model;
using PlainDeck.Sdk.Utils;

namespace PlainDeck.Sdk;

public sealed class Device
{
    private const int VendorId = 0x0FD9;

    private readonly HidDevice handle;
    private readonly DeviceConfiguration configuration;
    private readonly Dictionary<DeviceKey, KeyState> keypadState = new();

    public event EventHandler<DeviceKey>? OnKeyDown;
    public event EventHandler<DeviceKey>? OnKeyUp;

    private Device(HidDevice handle, DeviceConfiguration configuration)
    {
        this.handle = handle;
        this.configuration = configuration;

        for (var row = 0; row < this.configuration.ButtonRows; row++)
        {
            for (var col = 0; col < this.configuration.ButtonColumns; col++)
            {
                var key = this.configuration.GetKey(row: row, column: col);

                keypadState[key] = KeyState.Up;
            }
        }
    }

    private async Task HandleEvents(ChannelReader<byte[]> messageReader, CancellationToken cancellationToken)
    {
        await foreach (var message in messageReader.ReadAllAsync(cancellationToken))
        {
            var header = message[..4];
            if (header is [0x01, 0x00, var buttonCount, _] && buttonCount == Keys.Count())
            {
                var buttonData = message[4..];
                foreach (var (key, currentState) in keypadState)
                {
                    var updatedState = (KeyState)buttonData[key.Id];
                    if (currentState != updatedState)
                    {
                        var handler = updatedState switch
                        {
                            KeyState.Up => OnKeyUp,
                            KeyState.Down => OnKeyDown,
                            _ => null
                        };
                    
                        keypadState[key] = updatedState;
                        handler?.Invoke(this, key);
                    }
                }
            }
        }
    }
    
    public IEnumerable<DeviceKey> Keys => keypadState.Keys;

    public async Task ListenAsync(CancellationToken cancellationToken)
    {
        var messageChannel = Channel.CreateUnbounded<byte[]>();
        _ = HandleEvents(messageChannel.Reader, cancellationToken);
        
        await using var stream = handle.Open();
        stream.ReadTimeout = Timeout.Infinite;
        
        const int messageSize = 1024;
        var buffer = new byte[messageSize];
        while (!cancellationToken.IsCancellationRequested)
        {
            var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            await messageChannel.Writer.WriteAsync(buffer[..bytesRead], cancellationToken);
        }

        messageChannel.Writer.Complete();
    }
    
    public void SetKey(DeviceKey key, [StringSyntax("XML")] string svg)
    {
        const int setImageLength = 1024;
        const int headerLength = 8;

        var imageData = ImageUtils.SvgToJpeg(svg: svg, width: configuration.ButtonWidth, height: configuration.ButtonHeight);
        var counter = 0;
        
        using var stream = handle.Open();
        foreach (var chunk in imageData.Chunk(setImageLength - headerLength).Append([]))
        {
            var message = new byte[setImageLength];
            using var messageStream = new MemoryStream(message);
            messageStream.WriteByte(0x02);
            messageStream.WriteByte(0x07);
            messageStream.WriteByte(key.Id);
            messageStream.WriteByte(chunk.Length is 0 ? (byte)0x01 : (byte)0x00);
            messageStream.Write(chunk.Length.ToLittleEndian());
            messageStream.Write(counter.ToLittleEndian());
            messageStream.Write(chunk);
            
            stream.Write(message);

            counter += 1;
        }
    }

    public void SetBrightness(double brightness)
    {
        using var stream = handle.Open();
        var value = Math.Clamp(brightness, 0, 1) * 100;

        var brightnessRequest = new byte[32];
        brightnessRequest[0] = 0x03;
        brightnessRequest[1] = 0x08;
        brightnessRequest[2] = byte.CreateTruncating(value);

        stream.SetFeature(brightnessRequest);
    }

    public static Device Connect()
    {
        var devices = DeviceList.Local.GetHidDevices(VendorId).ToArray();
        if (devices is not [var firstDevice, ..])
        {
            throw new InvalidOperationException($"No device found using VID:{VendorId}.");
        }

        var configuration = DeviceConfiguration.Resolve(firstDevice.ProductID);
        if (configuration is null)
        {
            throw new InvalidOperationException($"Unknown device using PID:{firstDevice.ProductID}.");
        }

        return new Device(firstDevice, configuration);
    }
}
