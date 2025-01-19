using PlainDeck.Extensions;
using PlainDeck.Svg;

namespace Keypad;

public sealed class KeyHandler : ProxyKeyHandler
{
    public KeyHandler(string key, string? icon) : base(TranslateKey(key))
    {
        if (icon is not null)
        {
            this.WithSvgFromPath(icon);
        }
        else
        {
            this.WithImage(
                $"""
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16">
                    <rect x="0" y="0" width="16" height="16" fill="black" />
                    <text x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-family="sans-serif" font-size="8px" fill="white">{key}</text>
                </svg>
                """);
        }
    }

    private static ConsoleKey TranslateKey(string key) => Enum.Parse<ConsoleKey>(key);
}
