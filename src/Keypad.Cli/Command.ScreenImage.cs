using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record ScreenImage(string Path) : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            var image = new DeviceImage.File(Path);
            device.SetImage(image);

            return 0;
        }
    }
}
