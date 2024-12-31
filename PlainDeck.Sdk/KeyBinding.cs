using PlainDeck.Sdk.Model;

namespace PlainDeck.Sdk;

public static class KeyBinding
{
    public static void BindKey(this Device device, DeviceKey key, params ConsoleKey[] emittedKeys)
    {
        device.OnKeyDown += (_, pressedKey) =>
        {
            if (pressedKey == key)
            {
                foreach (var singleKey in emittedKeys)
                {
                    KeyEmitter.KeyDown(singleKey);
                }
            }
        };

        device.OnKeyUp += (_, pressedKey) =>
        {
            if (pressedKey == key)
            {
                foreach (var singleKey in emittedKeys.Reverse())
                {
                    KeyEmitter.KeyDown(singleKey);
                }
            }
        };
    }
}
