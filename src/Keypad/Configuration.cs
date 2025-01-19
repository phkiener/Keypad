namespace Keypad;

public sealed class Configuration
{
    public KeyConfiguration[] StreamDeckXL2022 { get; init; } = [];
    public KeyConfiguration[] StreamDeckPedal { get; init; } = [];
}

public readonly record struct KeyConfiguration(int Row, int Column, string Key, string? Icon);
