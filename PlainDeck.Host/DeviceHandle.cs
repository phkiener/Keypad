using System.Threading.Channels;
using HidSharp;

namespace PlainDeck.Host;

public abstract record Message;

public sealed record UndefinedMessage(byte[] Payload) : Message
{
    public override string ToString() => Convert.ToHexString(Payload);
}

public sealed class DeviceHandle
{
    private const int BufferSize = 1024;
    private readonly HidDevice device;

    private DeviceHandle(HidDevice device)
    {
        this.device = device;
    }
    
    private const int vendorId = 0x0FD9;
    private const int productId = 0x008F;

    public async Task ListenAsync(CancellationToken cancellationToken = default)
    {
        var messageChannel = Channel.CreateUnbounded<Message>();
        _ = HandleEvents(messageChannel.Reader, cancellationToken);

        await using var stream = device.Open();
        stream.ReadTimeout = Timeout.Infinite;

        var buffer = new byte[BufferSize];
        while (!cancellationToken.IsCancellationRequested)
        {
            var bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            var message = new UndefinedMessage(buffer[..bytesRead]);

            await messageChannel.Writer.WriteAsync(message, cancellationToken);
        }

        messageChannel.Writer.Complete();
    }

    private static async Task HandleEvents(ChannelReader<Message> messageReader, CancellationToken cancellationToken)
    {
        await foreach (var message in messageReader.ReadAllAsync(cancellationToken))
        {
            _ = message;
            
            Console.WriteLine($"Got message: {message}");
        }
    }

    public static DeviceHandle Find()
    {
        var device = DeviceList.Local.GetHidDeviceOrNull(vendorId, productId);
        if (device is null)
        {
            throw new InvalidOperationException("No device found");
        }

        return new DeviceHandle(device);
    }
}
