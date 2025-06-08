using Keypad.Core;
using Keypad.Core.Abstractions;

namespace Keypad.Cli;

public abstract partial record Command
{
    public DeviceType? DeviceType { get; set; }
    public string? SerialNumber { get; set; }

    public abstract int Invoke(ConnectedDevice device);
}
