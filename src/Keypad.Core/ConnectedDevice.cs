using HidSharp;
using Keypad.Core.Abstractions;

namespace Keypad.Core;

/// <summary>
/// A connected device that is listening to any interactions
/// </summary>
public abstract class ConnectedDevice : IDisposable, IAsyncDisposable
{
    private readonly CancellationTokenSource listenCancelled = new();
    private readonly Task listeningTask;
    
    /// <summary>
    /// Instantiate the <see cref="ConnectedDevice"/> and begin listening
    /// </summary>
    /// <param name="device">The underlying <see cref="HidDevice"/> to connect to</param>
    protected ConnectedDevice(HidDevice device)
    {
        Device = device;
        listeningTask = Task.Run(ListenAsync);
    }
    
    /// <summary>
    /// The serial number of the device
    /// </summary>
    public string SerialNumber => Device.GetSerialNumber();
    
    /// <summary>
    /// <see cref="DeviceType"/> of the device
    /// </summary>
    public DeviceType DeviceType => (DeviceType)Device.ProductID;
    
    /// All buttons that are present on the device
    public abstract IEnumerable<DeviceButton> Buttons { get; }

    /// <summary>
    /// Invoked whenever a key is being pressed.
    /// </summary>
    public event EventHandler<DeviceButton>? KeyPressed;
    
    /// <summary>
    /// Invoked whenever a key is released.
    /// </summary>
    public event EventHandler<DeviceButton>? KeyReleased;

    /// <summary>
    /// Put the device to sleep
    /// </summary>
    /// <returns><c>true</c> if the device has been put to sleep, <c>false</c> if the device does not support sleeping</returns>
    public virtual bool Sleep()
    {
        return false;
    }
    
    /// <summary>
    /// Wake the device from sleep
    /// </summary>
    /// <returns><c>true</c> if the device was awoken from sleep, <c>false</c> if the device does not support sleeping</returns>
    public virtual bool Wake()
    {
        return false;
    }
    
    /// <summary>
    /// Set the brightness for the device
    /// </summary>
    /// <param name="brightness">Target brightness; valid range is [0;1]</param>
    /// <returns><c>true</c> if the brightness was applied, <c>false</c> if the device does not support brightness</returns>
    public virtual bool SetBrightness(double brightness)
    {
        return false;
    }

    /// <summary>
    /// Set the image to display on a certain button
    /// </summary>
    /// <param name="button">The button whose image to set</param>
    /// <param name="image">The image to set on the button</param>
    /// <returns><c>true</c> if the image was applied, <c>false</c> if the device does not support setting button images</returns>
    public virtual bool SetImage(DeviceButton button, DeviceImage image)
    {
        return false;
    }

    /// <summary>
    /// Set an image for the whole screen of the device
    /// </summary>
    /// <param name="image">The image to set on the button</param>
    /// <returns><c>true</c> if the image was applied, <c>false</c> if the device does not support setting screen images</returns>
    public virtual bool SetImage(DeviceImage image)
    {
        return false;
    }
    
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
    /// The underlying <see cref="HidDevice"/>
    /// </summary>
    protected HidDevice Device { get; }
    
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
        await using var readStream = Device.Open();
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
