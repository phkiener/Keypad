namespace PlainDeck.Sdk.Extensions;

public partial class ProxyKeyHandler(ConsoleKey key) : KeyHandler
{
    public override Task OnKeyDown(IDeviceContext context)
    {
        if (OperatingSystem.IsWindows())
        {
            return KeyDown_Windows(key);
        }

        if (OperatingSystem.IsMacOS())
        {
            return KeyDown_MacOS(key);
        }

        throw new NotSupportedException($"Platform {Environment.OSVersion} is not supported");
    }

    public override Task OnKeyUp(IDeviceContext context)
    {
        if (OperatingSystem.IsWindows())
        {
            return KeyUp_Windows(key);
        }

        if (OperatingSystem.IsMacOS())
        {
            return KeyUp_MacOS(key);
        }

        throw new NotSupportedException($"Platform {Environment.OSVersion} is not supported");
    }
}
