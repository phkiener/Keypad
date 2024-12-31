using PlainDeck.Sdk.Model;

namespace PlainDeck.Sdk;

public static class KeyBinding
{
    public static void BindKey(this Device device, DeviceKey key, ConsoleKey emittedKey)
    {
        device.OnKeyDown += (_, pressedKey) =>
        {
            if (pressedKey == key)
            {
                KeyEmitter.KeyDown(emittedKey);
            }
        };

        device.OnKeyUp += (_, pressedKey) =>
        {
            if (pressedKey == key)
            {
                KeyEmitter.KeyUp(emittedKey);
            }
        };
    }
}
