using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record Timeout(double Delay, string ImagePath) : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            var image = new DeviceImage.File(ImagePath);
            var delay = TimeSpan.FromSeconds(Delay);

            device.SetScreensaver(image, delay);

            return 0;
        }
    }
}
