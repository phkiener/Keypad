using PlainDeck.Sdk.Model;

namespace PlainDeck.Sdk;

public sealed record DeviceConfiguration(int ButtonRows, int ButtonColumns, int ButtonWidth, int ButtonHeight)
{
    private const int StreamDeckXL2022 = 0x008F;
    private static readonly Dictionary<int, DeviceConfiguration> deviceConfigurationByProductId = new()
    {
        [StreamDeckXL2022] = new DeviceConfiguration(ButtonRows: 4, ButtonColumns: 8, ButtonWidth: 96, ButtonHeight: 96),
    };

    public DeviceKey GetKey(int row, int column)
    {
        var id = row * ButtonColumns + column;

        return new DeviceKey(row, column, byte.CreateTruncating(id));
    }

    public static DeviceConfiguration? Resolve(int productId) => deviceConfigurationByProductId.GetValueOrDefault(productId);
}
