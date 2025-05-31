using Keypad.Core;
using Keypad.Core.Device;

await using var device = DeviceManger.Connect(DeviceType.StreamDeckXL2022);
if (device is null)
{
    return 255;
}

device.KeyPressed  += (_, btn) => Console.WriteLine($"KEYDOWN {btn}");
device.KeyReleased += (_, btn) => Console.WriteLine($"KEYUP   {btn}");

var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellation.Cancel();

while (!cancellation.IsCancellationRequested)
{
    await Task.Delay(TimeSpan.FromSeconds(5), cancellation.Token);
}

return 0;
