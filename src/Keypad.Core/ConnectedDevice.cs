using HidSharp;
using Keypad.Core.Device;

namespace Keypad.Core;

/// <summary>
/// A connected device that is listening to any interactions
/// </summary>
public abstract class ConnectedDevice : IDisposable, IAsyncDisposable
{
    private readonly CancellationTokenSource listenCancelled = new();
    private readonly HidDevice device;
    private readonly Task listeningTask;
    
    /// <summary>
    /// Instantiate the <see cref="ConnectedDevice"/> and begin listening
    /// </summary>
    /// <param name="device">The underlying <see cref="HidDevice"/> to connect to</param>
    protected ConnectedDevice(HidDevice device)
    {
        this.device = device;
        listeningTask = Task.Run(ListenAsync);
    }
    
    public string SerialNumber => device.GetSerialNumber();
    public DeviceType DeviceType => (DeviceType)device.ProductID;

    /// <summary>
    /// Invoked whenever a key is being pressed.
    /// </summary>
    public event EventHandler<DeviceButton>? KeyPressed;
    
    /// <summary>
    /// Invoked whenever a key is released.
    /// </summary>
    public event EventHandler<DeviceButton>? KeyReleased;

    /// <summary>
    /// Receive a message from the device
    /// </summary>
    /// <param name="payload">The received payload</param>
    protected abstract void OnMessageReceived(ReadOnlySpan<byte> payload);

    /// <summary>
    /// Size of the message buffer; this should be big enough to fit all expected messages received from the device
    /// </summary>
    protected virtual int MessageBufferSize => 1024;

    /// <summary>
    /// Emit <see cref="KeyPressed"/> for the given button
    /// </summary>
    /// <param name="button">The <see cref="DeviceButton"/> that is being pressed</param>
    protected void OnKeyPressed(DeviceButton button)
    {
        KeyPressed?.Invoke(this, button);
    }
    
    /// <summary>
    /// Emit <see cref="KeyReleased"/> for the given button
    /// </summary>
    /// <param name="button">The <see cref="DeviceButton"/> that was released</param>
    protected void OnKeyReleased(DeviceButton button)
    {
        KeyReleased?.Invoke(this, button);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        listenCancelled.Cancel();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await listenCancelled.CancelAsync().ConfigureAwait(false);
        await listeningTask.ConfigureAwait(false);
    }

    private async Task ListenAsync()
    {
        await using var readStream = device.Open();
        readStream.ReadTimeout = Timeout.Infinite;
        
        var readBuffer = new byte[MessageBufferSize];
        while (!listenCancelled.IsCancellationRequested)
        {
            var cancellationToken = listenCancelled.Token;
            try
            {
                var payloadSize = await readStream.ReadAsync(readBuffer, cancellationToken).ConfigureAwait(false);

                OnMessageReceived(readBuffer.AsSpan(start: 0, length: payloadSize));
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message).ConfigureAwait(false);
            }
        }
    }
}
