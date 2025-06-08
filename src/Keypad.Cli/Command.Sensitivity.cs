using Keypad.Core;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record Sensitivity(double Value) : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            device.Wake();

            return 0;
        }
    }
}
