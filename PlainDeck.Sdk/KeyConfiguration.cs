using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PlainDeck.Sdk.Model;

namespace PlainDeck.Sdk;

public sealed class KeyConfiguration(DeviceKey deviceKey, Device device)
{
    public KeyConfiguration BindKey(ConsoleKey key)
    {
        device.OnKeyDown += (_, pressedKey) =>
        {
            if (pressedKey == deviceKey)
            {
                KeyEmitter.KeyDown(key);
            }
        };

        device.OnKeyUp += (_, pressedKey) =>
        {
            if (pressedKey == deviceKey)
            {
                KeyEmitter.KeyUp(key);
            }
        };
        
        return this;
    }

    public KeyConfiguration SetIcon([StringSyntax("XML")] string svg)
    {
        device.SetKey(deviceKey, svg);

        return this;
    }

    public KeyConfiguration SetIconFromResource(string path)
    {
        var resourceAssembly = Assembly.GetCallingAssembly();
        using var resourceStream = resourceAssembly.GetManifestResourceStream(path);
        if (resourceStream is null)
        {
            throw new ArgumentException($"No resource found at '{path}'.", nameof(path));
        }

        using var reader = new StreamReader(resourceStream);
        return SetIcon(reader.ReadToEnd());
    }
}
