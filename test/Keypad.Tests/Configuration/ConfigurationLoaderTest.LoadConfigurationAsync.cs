using Keypad.Configuration;

namespace Keypad.Tests.Configuration;

public sealed partial class ConfigurationLoaderTest
{
    [Test]
    public async Task ParsesEmptyConfiguration()
    {
        await using var stream = BuildConfigFile(@"{ ""devices"": [] }");

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        await Assert.That(parsedConfiguration.Devices).IsEmpty();
    }

    [Test]
    public async Task ParsesMinimalConfiguration()
    {
        await using var stream = BuildConfigFile(
            """
            { 
              "devices": [
                {
                  "type": "StreamDeck XL 2022",
                  "keys": []
                }
              ]
            }
            """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var deviceConfiguration = await Assert.That(parsedConfiguration.Devices).HasSingleItem();
        await Assert.That(deviceConfiguration?.Type).IsNotNull().IsEqualTo("StreamDeck XL 2022");
        await Assert.That(deviceConfiguration?.SerialNumber).IsNull();
        await Assert.That(deviceConfiguration?.Brightness).IsNotNull().IsEqualTo(0.7);
        await Assert.That(deviceConfiguration?.Keys).IsNotNull().IsEmpty();
    }

    [Test]
    public async Task Throws_WhenParsingGarbage()
    {
        await using var stream = BuildConfigFile("garbage");

        await Assert.ThrowsAsync<FormatException>(() => ConfigurationLoader.LoadConfigurationAsync(stream));
    }

    [Test]
    public async Task ParsesOptionalDeviceConfiguration()
    {
        await using var stream = BuildConfigFile(
            """
            { 
              "devices": [
                {
                  "type": "StreamDeck XL 2022",
                  "serial": "ABCDEFG",
                  "brightness": 0.3,
                  "keys": []
                }
              ]
            }
            """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var deviceConfiguration = await Assert.That(parsedConfiguration.Devices).HasSingleItem();
        await Assert.That(deviceConfiguration?.Type).IsNotNull().IsEqualTo("StreamDeck XL 2022");
        await Assert.That(deviceConfiguration?.SerialNumber).IsNotNull().IsEqualTo("ABCDEFG");
        await Assert.That(deviceConfiguration?.Brightness).IsNotNull().IsEqualTo(0.3);
        await Assert.That(deviceConfiguration?.Keys).IsNotNull().IsEmpty();
    }
}