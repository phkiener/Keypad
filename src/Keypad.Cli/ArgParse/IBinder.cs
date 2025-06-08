namespace Keypad.Cli.ArgParse;

public interface IBinder
{
    bool CanBind(Type targetType);
    
    bool TryBind(string value, Type targetType, out object? result);
}
