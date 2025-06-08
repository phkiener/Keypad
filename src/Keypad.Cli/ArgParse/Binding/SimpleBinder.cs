namespace Keypad.Cli.ArgParse.Binding;

public sealed class SimpleBinder<T>(SimpleBinder<T>.TryParse parser) : IBinder where T : struct
{
    public delegate bool TryParse(string text, out T value);

    public bool CanBind(Type targetType) => targetType == typeof(T) || targetType == typeof(T?);

    public bool TryBind(string value, Type targetType, out object? result)
    {
        if (string.IsNullOrWhiteSpace(value) && targetType == typeof(T?))
        {
            result = null;
            return true;
        }

        if (parser.Invoke(value, out var parsed))
        {
            result = parsed;
            return true;
        }

        result = null;
        return false;
    }
}
