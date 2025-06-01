using Keypad.Configuration;
using Keypad.Core.Abstractions;

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
        await Assert.That(deviceConfiguration?.Type).IsNotNull().IsEqualTo(DeviceType.StreamDeckXL2022);
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
        await Assert.That(deviceConfiguration?.Type).IsNotNull().IsEqualTo(DeviceType.StreamDeckXL2022);
        await Assert.That(deviceConfiguration?.SerialNumber).IsNotNull().IsEqualTo("ABCDEFG");
        await Assert.That(deviceConfiguration?.Brightness).IsNotNull().IsEqualTo(0.3);
        await Assert.That(deviceConfiguration?.Keys).IsNotNull().IsEmpty();
    }
    
    [Test]
    public async Task ParsesSimpleKey()
    {
        await using var stream = BuildConfigFile(
            """
            { 
              "devices": [
                {
                  "type": "StreamDeck XL 2022",
                  "keys": [
                    {
                      "key": "A",
                      "button": "2;3",
                      "image": "color:blue"
                    }
                  ]
                }
              ]
            }
            """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var key = parsedConfiguration.Devices.ElementAtOrDefault(0)?.Keys.ElementAtOrDefault(0);

        await Assert.That(key).IsNotNull();
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new EmulatedKey(EmulatedKey.Keycode.A));
        await Assert.That(key?.Button).IsNotNull().IsEqualTo(new DeviceButton(Row: 2, Column: 3));
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new DeviceImage.Color("blue"));
    }
    
    [Test]
    [Arguments("A", EmulatedKey.Keycode.A)]
    [Arguments("B", EmulatedKey.Keycode.B)]
    [Arguments("C", EmulatedKey.Keycode.C)]
    [Arguments("D", EmulatedKey.Keycode.D)]
    [Arguments("E", EmulatedKey.Keycode.E)]
    [Arguments("F", EmulatedKey.Keycode.F)]
    [Arguments("G", EmulatedKey.Keycode.G)]
    [Arguments("H", EmulatedKey.Keycode.H)]
    [Arguments("I", EmulatedKey.Keycode.I)]
    [Arguments("J", EmulatedKey.Keycode.J)]
    [Arguments("K", EmulatedKey.Keycode.K)]
    [Arguments("L", EmulatedKey.Keycode.L)]
    [Arguments("M", EmulatedKey.Keycode.M)]
    [Arguments("N", EmulatedKey.Keycode.N)]
    [Arguments("O", EmulatedKey.Keycode.O)]
    [Arguments("P", EmulatedKey.Keycode.P)]
    [Arguments("Q", EmulatedKey.Keycode.Q)]
    [Arguments("R", EmulatedKey.Keycode.R)]
    [Arguments("S", EmulatedKey.Keycode.S)]
    [Arguments("T", EmulatedKey.Keycode.T)]
    [Arguments("U", EmulatedKey.Keycode.U)]
    [Arguments("V", EmulatedKey.Keycode.V)]
    [Arguments("W", EmulatedKey.Keycode.W)]
    [Arguments("X", EmulatedKey.Keycode.X)]
    [Arguments("Y", EmulatedKey.Keycode.Y)]
    [Arguments("Z", EmulatedKey.Keycode.Z)]
    public async Task ParsesPlainKeyCodes(string givenCode, EmulatedKey.Keycode expectedKey)
    {
        await using var stream = BuildConfigFile(
            $$"""
            { 
              "devices": [
                {
                  "type": "StreamDeck XL 2022",
                  "keys": [
                    {
                      "key": "{{givenCode}}",
                      "button": "1;1",
                      "image": "color:blue"
                    }
                  ]
                }
              ]
            }
            """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var key = parsedConfiguration.Devices.ElementAtOrDefault(0)?.Keys.ElementAtOrDefault(0);

        await Assert.That(key).IsNotNull();
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new EmulatedKey(expectedKey));
    }
    
    [Test]
    public async Task ParsesKeyCombination()
    {
        await using var stream = BuildConfigFile(
            """
              { 
                "devices": [
                  {
                    "type": "StreamDeck XL 2022",
                    "keys": [
                      {
                        "key": "Shift+Control+Command+Option+A",
                        "button": "1;1",
                        "image": "color:blue"
                      }
                    ]
                  }
                ]
              }
              """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var key = parsedConfiguration.Devices.ElementAtOrDefault(0)?.Keys.ElementAtOrDefault(0);

        await Assert.That(key).IsNotNull();
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new EmulatedKey(EmulatedKey.Keycode.A) { Shift = true, Command = true, Control = true, Option = true });
    }
    
    [Test]
    public async Task ParsesHexColorImage()
    {
        await using var stream = BuildConfigFile(
            """
              { 
                "devices": [
                  {
                    "type": "StreamDeck XL 2022",
                    "keys": [
                      {
                        "key": "A",
                        "button": "1;1",
                        "image": "color:#ffab01"
                      }
                    ]
                  }
                ]
              }
              """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var key = parsedConfiguration.Devices.ElementAtOrDefault(0)?.Keys.ElementAtOrDefault(0);

        await Assert.That(key).IsNotNull();
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new DeviceImage.Color("#ffab01"));
    }
    
    [Test]
    public async Task ParsesFilePathImage()
    {
        await using var stream = BuildConfigFile(
            """
              { 
                "devices": [
                  {
                    "type": "StreamDeck XL 2022",
                    "keys": [
                      {
                        "key": "A",
                        "button": "1;1",
                        "image": "file:/some/path/to/image.png"
                      }
                    ]
                  }
                ]
              }
              """);

        var parsedConfiguration = await ConfigurationLoader.LoadConfigurationAsync(stream);
        var key = parsedConfiguration.Devices.ElementAtOrDefault(0)?.Keys.ElementAtOrDefault(0);

        await Assert.That(key).IsNotNull();
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new DeviceImage.File("/some/path/to/image.png"));
    }
}
