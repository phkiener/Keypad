using System.Text.Json.Serialization;
using Keypad.Configuration.Converters;
using Keypad.Core.Abstractions;

namespace Keypad.Tests.Configuration.Converters;

public sealed class EmulatedKeyConverterTest : JsonConverterTestBase<EmulatedKey>
{
    public override JsonConverter<EmulatedKey> Converter { get; } = new EmulatedKeyConverter();

    [Test]
    public async Task SimpleKeyCanBeConverted()
    {
        foreach (var letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            var expectedKeycode = EmulatedKey.Keycode.A + (letter - 'A');
            
            await AssertConversion($"{letter}", new EmulatedKey(expectedKeycode));
        }

        foreach (var number in "0123456789")
        {
            var expectedKeycode = EmulatedKey.Keycode.Key0 + (number - '0');
            var expectedNumKeycode = EmulatedKey.Keycode.Num0 + (number - '0');
            
            await AssertConversion($"{number}", new EmulatedKey(expectedKeycode));
            await AssertConversion($"Num{number}", new EmulatedKey(expectedNumKeycode));
        }

        foreach (var number in Enumerable.Range(1, 20))
        {
            var expectedKeycode = EmulatedKey.Keycode.F1 + (number - 1);
            await AssertConversion($"F{number}", new EmulatedKey(expectedKeycode));
        }
    }

    [Test]
    public async Task KeyWithModifiersCanBeConverted()
    {
        var baseKey = new EmulatedKey(EmulatedKey.Keycode.A);
        await AssertConversion("Shift+A", baseKey with { Shift = true });
        
        await AssertConversion("Ctrl+A", baseKey with { Control = true });
        await AssertConversion("Control+A", baseKey with { Control = true });
        
        await AssertConversion("Command+A", baseKey with { Command  = true });
        await AssertConversion("Cmd+A", baseKey with { Command = true });
        
        await AssertConversion("Option+A", baseKey with { Option = true });
        await AssertConversion("Opt+A", baseKey with { Option = true });
        
        await AssertConversion(
            "Shift+Command+Control+Option+A",
            baseKey with { Shift = true, Command = true, Control = true, Option = true });
    }
}
