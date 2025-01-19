namespace Keypad;

public sealed record Options(string ConfigFile)
{
    public bool Debug { get; init; } = false;
}
