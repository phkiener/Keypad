using Keypad.Core.Abstractions;
using Keypad.Core.Device;

namespace Keypad.Tests.Device;

public sealed class StreamDeckPedalTest : IDisposable
{
    private readonly TestableHidDevice underlyingDevice;
    private readonly StreamDeckPedal streamDeckPedal;

    public StreamDeckPedalTest()
    {
        underlyingDevice = new TestableHidDevice(0x0FD9, (int)DeviceType.StreamDeckPedal, "PEDAL");
        streamDeckPedal = new StreamDeckPedal(underlyingDevice);
    }

    [Test]
    public async Task UnsupportedFeaturesReturnFalse()
    {
        var featureResults = new[]
        {
            streamDeckPedal.SetBrightness(1),
            streamDeckPedal.SetImage(new DeviceImage.Color("black")),
            streamDeckPedal.SetImage(new DeviceButton(1, 1), new DeviceImage.Color("black")),
            streamDeckPedal.SetScreensaver(new DeviceImage.Color("black"), TimeSpan.FromSeconds(1)),
            streamDeckPedal.Sleep(),
            streamDeckPedal.Wake()
        };

        await Assert.That(featureResults).All().Satisfy(static x => x.IsFalse());
    }

    [Test]
    public async Task ReportsCorrectInformation()
    {
        await Assert.That(streamDeckPedal.DeviceType).IsEqualTo(DeviceType.StreamDeckPedal);
        await Assert.That(streamDeckPedal.SerialNumber).IsEqualTo("PEDAL");

        var expectedButtons = new DeviceButton[] { new(1, 1), new(1, 2), new(1, 3) };
        await Assert.That(streamDeckPedal.Buttons).IsEquivalentTo(expectedButtons);
    }

    [Test]
    public async Task SensitivityIsSet()
    {
        streamDeckPedal.SetSensitivity(-0.5);
        streamDeckPedal.SetSensitivity(0.0);
        streamDeckPedal.SetSensitivity(0.5);
        streamDeckPedal.SetSensitivity(0.7);
        streamDeckPedal.SetSensitivity(1);
        
        await Assert.That(underlyingDevice.WrittenFeatures[0]).Satisfies(static x => x is [0x02, 0x0A, 0, ..]);
        await Assert.That(underlyingDevice.WrittenFeatures[1]).Satisfies(static x => x is [0x02, 0x0A, 0, ..]);
        await Assert.That(underlyingDevice.WrittenFeatures[2]).Satisfies(static x => x is [0x02, 0x0A, 50, ..]);
        await Assert.That(underlyingDevice.WrittenFeatures[3]).Satisfies(static x => x is [0x02, 0x0A, 70, ..]);
        await Assert.That(underlyingDevice.WrittenFeatures[4]).Satisfies(static x => x is [0x02, 0x0A, 70, ..]);
    }

    public void Dispose()
    {
        streamDeckPedal.Dispose();
        underlyingDevice.Dispose();
    }
}
