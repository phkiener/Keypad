namespace PlainDeck.Sdk.Hosting;

public sealed partial class DeviceHost
{
    private readonly Dictionary<DeviceKey, KeyHandler> keyHandlers = new();
    private readonly Dictionary<DeviceKey, ButtonState> keyState = new();

    public void MapKey(int row, int column, KeyHandler handler)
    {
        var key = configuration.GetKey(row, column);
        MapKey(key, handler);
    }

    public void MapKey(DeviceKey key, KeyHandler handler)
    {
        keyHandlers[key] = handler;

        handler.Key = key;
    }

    public IEnumerable<DeviceKey> Keys => configuration.Keys;
}
