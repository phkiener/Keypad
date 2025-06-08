using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core.Abstractions;

namespace Keypad.Tests.Configuration.Converters;

public sealed class DeviceTypeConverterTest : JsonConverterTestBase<DeviceType>
{
    public override JsonConverter<DeviceType> Converter { get; } = new DeviceTypeConverter();

    [Test]
    public async Task StreamDeckXL2022CanBeParsed()
    {
        await AssertConversion("StreamDeck XL 2022", DeviceType.StreamDeckXL2022);
        await AssertConversion("StreamDeckXL2022", DeviceType.StreamDeckXL2022);
        await AssertConversion("XL2022", DeviceType.StreamDeckXL2022);
    }
    
    [Test]
    public async Task StreamDeckPedalCanBeParsed()
    {
        await AssertConversion("StreamDeck Pedal", DeviceType.StreamDeckPedal);
        await AssertConversion("StreamDeckPedal", DeviceType.StreamDeckPedal);
        await AssertConversion("Pedal", DeviceType.StreamDeckPedal);
    }
    
    [Test]
    public void UnsupportedDevicesCannotBeParsed()
    {
        AssertConversionFails("StreamDeck Mini Mk2");
        AssertConversionFails("Schnitzel");
    }
}
