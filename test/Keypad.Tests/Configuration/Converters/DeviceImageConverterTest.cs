using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core.Abstractions;

namespace Keypad.Tests.Configuration.Converters;

public sealed class DeviceImageConverterTest : JsonConverterTestBase<DeviceImage>
{
    public override JsonConverter<DeviceImage> Converter { get; } = new DeviceImageConverter();

    [Test]
    public async Task ColorImageCanBeParsed()
    {
        await AssertConversion("color:blue", new DeviceImage.Color("blue"));
        await AssertConversion("color:#ff00aa", new DeviceImage.Color("#ff00aa"));
        await AssertConversion("color:#AABBCC", new DeviceImage.Color("#AABBCC"));
        await AssertConversion("color:chucknorrisred", new DeviceImage.Color("chucknorrisred"));
        await AssertConversion("color:rgb(10, 255, 0)", new DeviceImage.Color("rgb(10, 255, 0)"));
    }
    
    [Test]
    public async Task FileImageCanBeParsed()
    {
        await AssertConversion("file:./image.png", new DeviceImage.File("./image.png"));
        await AssertConversion("file:/full/path/to/image.jpg", new DeviceImage.File("/full/path/to/image.jpg"));
    }
    
    [Test]
    public void InvalidImageCannotBeParsed()
    {
        AssertConversionFails("red");
        AssertConversionFails("./image.png");
        AssertConversionFails("not-color:abcd");
        AssertConversionFails("colour:pinkque");
        AssertConversionFails("path:file.xml");
    }
}
