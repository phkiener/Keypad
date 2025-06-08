using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core.Abstractions;

namespace Keypad.Tests.Configuration.Converters;

public sealed class DeviceButtonConverterTest : JsonConverterTestBase<DeviceButton>
{
    public override JsonConverter<DeviceButton> Converter { get; } = new DeviceButtonConverter();
    
    [Test]
    public async Task ValidButtonsCanBeParsed()
    {
        await AssertConversion("1;1", new DeviceButton(Row: 1, Column: 1));
        await AssertConversion("8;1", new DeviceButton(Row: 8, Column: 1));
        await AssertConversion("1;8", new DeviceButton(Row: 1, Column: 8));
    }
    [Test]
    public void InvalidButtonsCannotBeParsed()
    {
        AssertConversionFails("");
        AssertConversionFails("1-2");
        AssertConversionFails("1:2");
        AssertConversionFails("-1;-1");
        AssertConversionFails("0;0");
        AssertConversionFails("10;10");
    }
}
