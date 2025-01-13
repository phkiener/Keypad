using HidSharp;

namespace PlainDeck.Sdk;

public sealed class DeviceContext(HidDevice device, DeviceConfiguration deviceConfiguration)
{
    public void SetBrightness(double percentage)
    {
        if (!deviceConfiguration.HasBrightness)
        {
            return;
        }
        
        using var stream = device.Open();
        var value = Math.Clamp(percentage, 0, 1) * 100;

        var brightnessRequest = new byte[32];
        brightnessRequest[0] = 0x03;
        brightnessRequest[1] = 0x08;
        brightnessRequest[2] = byte.CreateTruncating(value);

        stream.SetFeature(brightnessRequest);
    }
}
