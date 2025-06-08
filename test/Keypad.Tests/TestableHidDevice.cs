using HidSharp;

namespace Keypad.Tests;

public sealed class TestableHidDevice(int vendorId, int productId, string serialNumber) : HidDevice, IDisposable
{
    private readonly List<byte[]> writtenFeatures = [];
    private readonly List<byte[]> writtenMessages = [];
    private readonly MemoryStream featureReadStream = new();
    private readonly MemoryStream contentReadStream = new();
    
    public override int VendorID => vendorId;
    public override int ProductID => productId;
    public override int ReleaseNumberBcd => 0;
    public override string DevicePath => "TEST";
    
    public override string GetManufacturer() => "TEST";
    public override string GetProductName() => "TEST";
    public override string GetSerialNumber() => serialNumber;
    public override string GetFileSystemName() => "/dev/usb/test";
    
    public override int GetMaxInputReportLength() => int.MaxValue;
    public override int GetMaxOutputReportLength() => int.MaxValue;
    public override int GetMaxFeatureReportLength() => int.MaxValue;

    public IReadOnlyList<byte[]> WrittenFeatures => writtenFeatures;
    public IReadOnlyList<byte[]> WrittenMessages => writtenMessages;

    public void WriteContent(byte[] buffer)
    {
        contentReadStream.Write(buffer, 0, buffer.Length);
        contentReadStream.Position -= buffer.Length;
    }
    
    public void WriteFeature(byte[] buffer)
    {
        featureReadStream.Write(buffer, 0, buffer.Length);
        featureReadStream.Position -= buffer.Length;
    }
    
    protected override DeviceStream OpenDeviceDirectly(OpenConfiguration openConfig)
    {
        return new TestableDeviceStream(device: this, readTimeout: 0, writeTimeout: 0);
    }

    private sealed class TestableDeviceStream(TestableHidDevice device, int readTimeout, int writeTimeout) : HidStream(device)
    {
        public override int ReadTimeout { get; set; } = readTimeout;
        public override int WriteTimeout { get; set; } = writeTimeout;

        public override void GetFeature(byte[] buffer, int offset, int count)
        {
            _ = device.featureReadStream.Read(buffer, offset, count);
        }

        public override void SetFeature(byte[] buffer, int offset, int count)
        {
            using var memoryBuffer = new MemoryStream();
            memoryBuffer.Write(buffer, offset, count);

            device.writtenFeatures.Add(memoryBuffer.ToArray());
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return device.contentReadStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            using var memoryBuffer = new MemoryStream();
            memoryBuffer.Write(buffer, offset, count);

            device.writtenMessages.Add(memoryBuffer.ToArray());
        }
        
        public override void Flush()
        {
            device.featureReadStream.Flush();
            device.contentReadStream.Flush();
        }
    }

    public void Dispose()
    {
        featureReadStream.Dispose();
        contentReadStream.Dispose();
    }
}
