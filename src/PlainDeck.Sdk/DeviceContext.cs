using HidSharp;
using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Sdk;

public sealed class DeviceContext(HidDevice device, DeviceConfiguration deviceConfiguration)
{
    public DeviceConfiguration Device { get; } = deviceConfiguration;

    public void SetBrightness(double percentage)
    {
        if (!Device.HasBrightness)
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

    public void SetKeyImage(DeviceKey key, byte[] imageData)
    {
        if (!Device.HasKeyImage)
        {
            return;
        }
        
        const int setImageLength = 1024;
        const int headerLength = 8;
        
        var counter = 0;
        
        using var stream = device.Open();
        foreach (var chunk in imageData.Chunk(setImageLength - headerLength).Append([]))
        {
            var message = new byte[setImageLength];
            using var messageStream = new MemoryStream(message);
            messageStream.WriteByte(0x02);
            messageStream.WriteByte(0x07);
            messageStream.WriteByte(key.Id);
            messageStream.WriteByte(chunk.Length is 0 ? (byte)0x01 : (byte)0x00);
            messageStream.Write(ToLittleEndian(chunk.Length));
            messageStream.Write(ToLittleEndian(counter));
            messageStream.Write(chunk);
            
            stream.Write(message);

            counter += 1;
        }
    }
    
    private static byte[] ToLittleEndian(int value) => [(byte)(value & 0xFF), (byte)((value >> 8) & 0xFF)];
}
