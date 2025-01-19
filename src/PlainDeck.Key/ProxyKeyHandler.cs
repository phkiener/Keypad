namespace PlainDeck.Extensions;

public class ProxyKeyHandler(ConsoleKey key) : KeyHandler
{
    public override Task OnKeyDown(IDeviceContext context)
    {
        KeyPress.KeyDown(key);

        return Task.CompletedTask;
    }

    public override Task OnKeyUp(IDeviceContext context)
    {
        KeyPress.KeyUp(key);

        return Task.CompletedTask;
    }
}
