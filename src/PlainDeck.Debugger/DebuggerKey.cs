using PlainDeck.Sdk;
using PlainDeck.Sdk.Extensions;
using PlainDeck.Sdk.Svg;

namespace PlainDeck.Debugger;

public sealed class DebuggerKey(DeviceType type) : KeyHandler
{
    public override Task OnBind(IDeviceContext context)
    {
        context.SetKeyImage(Key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='blue'/></svg>");
        Console.WriteLine($"{type} ({Key.Row} / {Key.Column}): OnBind");

        return Task.CompletedTask;
    }

    public override Task OnKeyUp(IDeviceContext context)
    {
        context.SetKeyImage(Key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='blue'/></svg>");
        Console.WriteLine($"{type} ({Key.Row} / {Key.Column}): OnKeyUp");

        return Task.CompletedTask;
    }

    public override Task OnKeyDown(IDeviceContext context)
    {
        context.SetKeyImage(Key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='white'/></svg>");
        Console.WriteLine($"{type} ({Key.Row} / {Key.Column}): OnKeyDown");

        return Task.CompletedTask;
    }
}
