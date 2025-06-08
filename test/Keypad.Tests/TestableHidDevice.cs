using HidSharp;

namespace Keypad.Tests;

public sealed class TestableHidDevice(int vendorId, int productId, string serialNumber) : HidDevice
{
    private readonly List<byte[]> writtenFeatures = [];
    private readonly List<byte[]> writtenMessages = [];
    private readonly Queue<byte[]> availableMessages = [];
    
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
        availableMessages.Enqueue(buffer);
    }

    public async Task FlushAsync()
    {
        while (availableMessages.Count > 0)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(2));
        }
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
            throw new NotSupportedException();
        }

        public override void SetFeature(byte[] buffer, int offset, int count)
        {
            using var memoryBuffer = new MemoryStream();
            memoryBuffer.Write(buffer, offset, count);

            device.writtenFeatures.Add(memoryBuffer.ToArray());
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (device.availableMessages.TryDequeue(out var readBuffer))
            {
                using var memoryBuffer = new MemoryStream(readBuffer);
                return memoryBuffer.Read(buffer, offset, count);
            }

            return 0;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            using var memoryBuffer = new MemoryStream();
            memoryBuffer.Write(buffer, offset, count);

            device.writtenMessages.Add(memoryBuffer.ToArray());
        }
        
        public override void Flush()
        {
        }
    }
}
