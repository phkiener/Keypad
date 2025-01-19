using PlainDeck.Hosting;

namespace PlainDeck;

public interface IDeviceContext
{
    DeviceConfiguration Device { get; }
    void SetBrightness(double percentage);
    void SetKeyImage(DeviceKey key, byte[] imageData);
    void SetScreensaver(byte[] imageData);
    void SetStandbyImage(byte[] imageData);

    void Sleep();
    void Wake();

    void SetSensitivity(double percentage);
    void SetStatusLed(byte red, byte green, byte blue);
}
