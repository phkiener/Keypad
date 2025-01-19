using YamlDotNet.Serialization;

namespace Keypad;

public sealed class Configuration
{
    public KeyConfiguration[] StreamDeckXL2022 { get; init; } = [];
    public KeyConfiguration[] StreamDeckPedal { get; init; } = [];

    public static Configuration ReadFrom(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);

        var serializer = new DeserializerBuilder().Build();
        return serializer.Deserialize<Configuration>(reader);
    }
}

public readonly record struct KeyConfiguration(int Row, int Column, string Key, string? Icon);
