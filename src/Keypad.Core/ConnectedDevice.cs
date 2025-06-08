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
    /// How many rows of buttons there are
    /// </summary>
    protected abstract int Rows { get; }
    
    /// <summary>
    /// How many columns of buttons there are
    /// </summary>
    protected abstract int Columns { get; }

    /// <summary>
    /// A dictionary that maps a button's index to a <see cref="DeviceButton"/>
    /// </summary>
    protected abstract Dictionary<byte, DeviceButton> ButtonIndex { get; }
    
    /// <summary>
    /// A dictionary containing the <see cref="ButtonState"/> for each <see cref="DeviceButton"/>
    /// </summary>
    protected abstract Dictionary<DeviceButton, ButtonState> Keymap { get; }

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
    public IEnumerable<DeviceButton> Buttons => ButtonIndex.Values;

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
    /// Set the sensitivity for the buttons
    /// </summary>
    /// <returns><c>true</c> if the sensitivity was applied, <c>false</c> if the device does not support setting sensitivity</returns>
    public virtual bool SetSensitivity(double sensitivity)
    {
        return false;
    }
    
    /// <summary>
    /// Set the color for the status LED
    /// </summary>
    /// <param name="color">The color to set as RRGGBB-hexcode (i.e. <c>FFAB01</c></param>
    /// <returns><c>true</c> if the color was applied, <c>false</c> if the device does not have a status LED</returns>
    public virtual bool SetStatusLED(string color)
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
    /// <param name="image">The image to set on the device</param>
    /// <returns><c>true</c> if the image was applied, <c>false</c> if the device does not support setting screen images</returns>
    public virtual bool SetImage(DeviceImage image)
    {
        return false;
    }

    /// <summary>
    /// Set an image as screensaver
    /// </summary>
    /// <param name="image">The image to set as screensaver</param>
    /// <param name="delay">Delay after which to show the screen saver</param>
    /// <returns><c>true</c> if the image was applied, <c>false</c> if the device does not support setting a screensaver</returns>
    public virtual bool SetScreensaver(DeviceImage image, TimeSpan delay)
    {
        return false;
    }

    /// <summary>
    /// Receive a message from the device
    /// </summary>
    /// <param name="payload">The received payload</param>
    protected virtual void OnMessageReceived(ReadOnlySpan<byte> payload)
    {
        if (payload is [0x01, 0x00, var length, 0x00, ..] && length == Rows * Columns)
        {
            for (byte i = 0; i < Rows * Columns; i++)
            {
                var buttonState = (ButtonState)payload[4 + i];
                var deviceButton = ButtonIndex[i];
                
                var currentState = Keymap[deviceButton];
                if (currentState == buttonState)
                {
                    continue;
                }

                Keymap[deviceButton] = buttonState;
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
