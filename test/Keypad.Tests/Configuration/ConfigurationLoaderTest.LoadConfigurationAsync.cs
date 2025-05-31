using Keypad.Configuration;
using Keypad.Core;
using Keypad.Core.Device;

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
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new KeypadKey(KeypadKey.Keycode.A));
        await Assert.That(key?.Button).IsNotNull().IsEqualTo(new DeviceButton(Row: 2, Column: 3));
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new KeypadImage.Color("blue"));
    }
    
    [Test]
    [Arguments("A", KeypadKey.Keycode.A)]
    [Arguments("B", KeypadKey.Keycode.B)]
    [Arguments("C", KeypadKey.Keycode.C)]
    [Arguments("D", KeypadKey.Keycode.D)]
    [Arguments("E", KeypadKey.Keycode.E)]
    [Arguments("F", KeypadKey.Keycode.F)]
    [Arguments("G", KeypadKey.Keycode.G)]
    [Arguments("H", KeypadKey.Keycode.H)]
    [Arguments("I", KeypadKey.Keycode.I)]
    [Arguments("J", KeypadKey.Keycode.J)]
    [Arguments("K", KeypadKey.Keycode.K)]
    [Arguments("L", KeypadKey.Keycode.L)]
    [Arguments("M", KeypadKey.Keycode.M)]
    [Arguments("N", KeypadKey.Keycode.N)]
    [Arguments("O", KeypadKey.Keycode.O)]
    [Arguments("P", KeypadKey.Keycode.P)]
    [Arguments("Q", KeypadKey.Keycode.Q)]
    [Arguments("R", KeypadKey.Keycode.R)]
    [Arguments("S", KeypadKey.Keycode.S)]
    [Arguments("T", KeypadKey.Keycode.T)]
    [Arguments("U", KeypadKey.Keycode.U)]
    [Arguments("V", KeypadKey.Keycode.V)]
    [Arguments("W", KeypadKey.Keycode.W)]
    [Arguments("X", KeypadKey.Keycode.X)]
    [Arguments("Y", KeypadKey.Keycode.Y)]
    [Arguments("Z", KeypadKey.Keycode.Z)]
    public async Task ParsesPlainKeyCodes(string givenCode, KeypadKey.Keycode expectedKey)
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
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new KeypadKey(expectedKey));
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
        await Assert.That(key?.Key).IsNotNull().IsEqualTo(new KeypadKey(KeypadKey.Keycode.A) { Shift = true, Command = true, Control = true, Option = true });
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
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new KeypadImage.Color("#ffab01"));
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
        await Assert.That(key?.Image).IsNotNull().IsEqualTo(new KeypadImage.File("/some/path/to/image.png"));
    }
}
