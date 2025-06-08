using Keypad.Core;

namespace Keypad.Cli;

public abstract partial record Command
{
    public sealed record StatusColor(string Color) : Command
    {
        public override int Invoke(ConnectedDevice device)
        {
            device.SetStatusLED(Color);

            return 0;
        }
    }
}
