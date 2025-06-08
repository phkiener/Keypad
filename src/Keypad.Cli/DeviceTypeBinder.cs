using Keypad.Cli.ArgParse;
using Keypad.Core.Abstractions;

namespace Keypad.Cli;

public class DeviceTypeBinder : IBinder
{
    public bool CanBind(Type targetType)
    {
        return targetType == typeof(DeviceType);
    }

    public bool TryBind(string value, Type targetType, out object? result)
    {
        return Enum.TryParse(targetType, value, ignoreCase: true, out result);
    }
}
