using PlainDeck.Configuration;
using PlainDeck.Hosting;

namespace PlainDeck;

public abstract class DeviceConfiguration
{
    public abstract int ButtonRows { get; }
    public abstract int ButtonColumns { get; }

    public virtual bool HasBrightness => false;
    public virtual bool HasKeyImage => false;
    public virtual int KeyImageWidth => throw new NotSupportedException("Device does not support key images");
    public virtual int KeyImageHeight => throw new NotSupportedException("Device does not support key images");
    public virtual bool KeyImageFlip => throw new NotSupportedException("Device does not support key images");
    public virtual string KeyImageFormat => throw new NotSupportedException("Device does not support key images");
    
    public DeviceKey GetKey(int row, int column) => new(Row: row, Column: column, Id: byte.CreateTruncating(row * ButtonColumns + column));
    public IEnumerable<DeviceKey> Keys => Enumerable.Range(0, ButtonRows).SelectMany(r => Enumerable.Range(0, ButtonColumns).Select(c => GetKey(r, c)));


    public static DeviceConfiguration Resolve(DeviceType type)
    {
        return type switch
        {
            DeviceType.StreamDeckXL2022 => new StreamDeckXL2022(),
            DeviceType.StreamDeckPedal => new StreamDeckPedal(),
            _ => throw new NotSupportedException()
        };
    }
}
