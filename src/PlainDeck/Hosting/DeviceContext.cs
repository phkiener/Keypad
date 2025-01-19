using HidSharp;

namespace PlainDeck.Hosting;

public sealed class DeviceContext(HidDevice device, DeviceConfiguration deviceConfiguration) : IDeviceContext
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

    public void SetTimeout(short seconds)
    {
        var brightnessRequest = new byte[32];
        brightnessRequest[0] = 0x03;
        brightnessRequest[1] = 0x0d;

        var timeout = ToLittleEndian(seconds);
        brightnessRequest[2] = timeout[0];
        brightnessRequest[3] = timeout[1];

        using var stream = device.Open();
        stream.SetFeature(brightnessRequest);
    }

    public void SetSensitivity(double percentage)
    {
        if (!Device.HasSensitivity)
        {
            return;
        }
        
        using var stream = device.Open();
        var value = Math.Clamp(percentage, 0, .7) * 100;

        var message = new byte[1024];
        message[0] = 0x02;
        message[1] = 0x0a;
        message[2] = 0x03;
        message[3] = byte.CreateTruncating(value);
        message[4] = 0;

        stream.Write(message);
    }

    public void SetStatusLed(byte red, byte green, byte blue)
    {
        if (!Device.HasStatusLed)
        {
            return;
        }
        using var stream = device.Open();

        var message = new byte[1024];
        message[0] = 0x02;
        message[1] = 0x0b;
        message[2] = red;
        message[3] = green;
        message[4] = blue;
        message[5] = 0x02;

        stream.Write(message);
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

    public void Sleep()
    {
        using var stream = device.Open();

        var message = new byte[32];
        message[0] = 0x03;
        message[1] = 0x02;

        stream.SetFeature(message);
    }
    public void Wake()
    {
        using var stream = device.Open();

        var message = new byte[32];
        message[0] = 0x03;
        message[1] = 0x05;

        stream.SetFeature(message);
    }

    /* I got these via wireshark, but something is off here */

    public void SetFullscreenImage(byte[] imageData)
    {
        if (!Device.HasKeyImage)
        {
            return;
        }
        
        const int setImageLength = 1024;
        const int headerLength = 8;
        const int chunkLength = setImageLength - headerLength;
        
        var counter = 0;
        using var stream = device.Open();
        foreach (var chunk in imageData.Chunk(chunkLength))
        {
            var message = new byte[setImageLength];
            using var messageStream = new MemoryStream(message);
            messageStream.WriteByte(0x02);
            messageStream.WriteByte(0x08);
            messageStream.WriteByte(0x00);
            messageStream.WriteByte(chunkLength * (counter + 1) >= imageData.Length ? (byte)0x01 : (byte)0x00);
            messageStream.Write(ToLittleEndian(chunk.Length));
            messageStream.Write(ToLittleEndian(counter));
            messageStream.Write(chunk);
            
            stream.Write(message);

            counter += 1;
        }
    }

    public void SetStandbyImage(byte[] imageData)
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
            messageStream.WriteByte(0x09);
            messageStream.WriteByte(0x08);
            messageStream.WriteByte(chunk.Length is 0 ? (byte)0x01 : (byte)0x00);
            messageStream.Write(ToLittleEndian(counter));
            messageStream.Write(ToLittleEndian(chunk.Length));
            messageStream.Write(chunk);
            
            stream.Write(message);

            counter += 1;
        }
    }
    
    // 0x03 0x0d A A ?????
    // A A: Timeout until screen saver, in seconds, little endian

    private static byte[] ToLittleEndian(int value) => [(byte)(value & 0xFF), (byte)((value >> 8) & 0xFF)];
}
