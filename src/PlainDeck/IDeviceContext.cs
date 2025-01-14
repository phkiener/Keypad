using PlainDeck.Hosting;

namespace PlainDeck;

public interface IDeviceContext
{
    DeviceConfiguration Device { get; }
    void SetBrightness(double percentage);
    void SetKeyImage(DeviceKey key, byte[] imageData);
}
