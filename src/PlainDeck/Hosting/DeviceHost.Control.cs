namespace PlainDeck.Hosting;

public sealed partial class DeviceHost : IDeviceContext
{
    public DeviceConfiguration Device => context.Device;
    public void SetBrightness(double percentage) => context.SetBrightness(percentage);
    public void SetKeyImage(DeviceKey key, byte[] imageData) => context.SetKeyImage(key, imageData);
    public void SetScreensaver(byte[] imageData) => context.SetScreensaver(imageData);
    public void SetStandbyImage(byte[] imageData) => context.SetStandbyImage(imageData);
    public void Sleep() => context.Sleep();
    public void Wake() => context.Wake();
    public void SetSensitivity(double percentage) => context.SetSensitivity(percentage);
    public void SetStatusLed(byte red, byte green, byte blue) => context.SetStatusLed(red, green, blue);
}
