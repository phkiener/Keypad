using Keypad.Cli.ArgParse.Binding;

namespace Keypad.Cli.ArgParse;

public static class Parser
{
    public static readonly ArgumentParser DefaultParser = new(
    [
        new StringBinder(),
        new SimpleBinder<byte>(byte.TryParse),
        new SimpleBinder<sbyte>(sbyte.TryParse),
        new SimpleBinder<short>(short.TryParse),
        new SimpleBinder<ushort>(ushort.TryParse),
        new SimpleBinder<int>(int.TryParse),
        new SimpleBinder<uint>(uint.TryParse),
        new SimpleBinder<long>(long.TryParse),
        new SimpleBinder<ulong>(ulong.TryParse),
        new SimpleBinder<float>(float.TryParse),
        new SimpleBinder<double>(double.TryParse),
        new SimpleBinder<decimal>(decimal.TryParse),
    ]);

    public static T Parse<T>(string[] args) => DefaultParser.Parse<T>(args);
}
