using Keypad.Core;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record Sleep : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            device.Sleep();

            return 0;
        }
    }
}
