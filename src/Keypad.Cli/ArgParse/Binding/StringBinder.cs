namespace Keypad.Cli.ArgParse.Binding;

public sealed class StringBinder : IBinder
{
    public bool CanBind(Type targetType) => targetType == typeof(string);

    public bool TryBind(string value, Type targetType, out object? result)
    {
        result = value;
        return true;
    }
}
