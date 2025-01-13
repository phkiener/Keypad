using System.Threading.Channels;
using HidSharp;

namespace PlainDeck.Sdk.Hosting;

public readonly record struct DeviceMessage(byte[] Payload);

public readonly record struct DeviceKey(int Row, int Column, byte Id);

public enum ButtonState { Unknown = 0, Down = 1, Up = -1 }

public sealed partial class DeviceHost(HidDevice device, DeviceConfiguration configuration)
{
    private readonly Channel<DeviceMessage> messageChannel = Channel.CreateUnbounded<DeviceMessage>();
    
    public async Task ListenAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = device.Open();
        stream.ReadTimeout = Timeout.Infinite;

        var context = await InitializeContextAsync();
        _ = ProcessMessagesAsync(context, cancellationToken);

        var incomingMessage = new byte[1024];
        while (!cancellationToken.IsCancellationRequested)
        {
            var payloadSize = await stream.ReadAsync(incomingMessage, cancellationToken).ConfigureAwait(false);
            var message = new DeviceMessage(incomingMessage[..payloadSize]);
            
            await messageChannel.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        }

        messageChannel.Writer.Complete();
    }

    private async Task<DeviceContext> InitializeContextAsync()
    {
        var context = new DeviceContext(device, configuration);
        
        foreach (var key in configuration.Keys)
        {
            keyState.Add(key, ButtonState.Unknown);
        }

        foreach (var handler in keyHandlers.Values)
        {
            await handler.OnBind(context).ConfigureAwait(false);
        }

        return context;
    }

    private async Task ProcessMessagesAsync(DeviceContext context, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await messageChannel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    if (message.Payload is [0x01, 0x00, var count, _, ..])
                    {
                        var buttonState = message.Payload[4..(count + 4)];

                        foreach (var (key, state) in keyState)
                        {
                            var currentState = buttonState[key.Id] is 1 ? ButtonState.Down : ButtonState.Up;
                            if (currentState == state)
                            {
                                continue;
                            }

                            var handler = keyHandlers.GetValueOrDefault(key);
                            if (currentState is ButtonState.Up)
                            {
                                keyState[key] = ButtonState.Up;
                                if (state is not ButtonState.Unknown)
                                {
                                    _ = handler?.OnKeyUp(context);
                                }
                            }
                        
                            if (currentState is ButtonState.Down)
                            {
                                keyState[key] = ButtonState.Down;
                                _ = handler?.OnKeyDown(context);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[ERROR] Unhandled exception: {e.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FATAL] Unhandled exception: {e.Message}");
        }
    }
}
