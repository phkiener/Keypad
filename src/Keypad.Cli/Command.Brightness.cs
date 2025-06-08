using Keypad.Core;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record Brightness(double Value) : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            device.SetBrightness(Value);

            return 0;
        }
    }
}
