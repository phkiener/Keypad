using PlainDeck.Hosting;

namespace PlainDeck;

public interface IDeviceContext
{
    DeviceConfiguration Device { get; }
    void SetBrightness(double percentage);
    void SetKeyImage(DeviceKey key, byte[] imageData);
    void SetFullscreenImage(byte[] imageData);
    void SetStandbyImage(byte[] imageData);

    void Sleep();
    void Wake();
    void SetTimeout(short seconds);

    void SetSensitivity(double percentage);
    void SetStatusLed(byte red, byte green, byte blue);
}
