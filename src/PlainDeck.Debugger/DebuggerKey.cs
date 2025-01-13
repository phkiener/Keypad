using PlainDeck.Sdk;
using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Debugger;

public sealed class DebuggerKey(DeviceType type, DeviceKey key) : KeyHandler
{
    public override Task OnBind(DeviceContext context)
    {
        Console.WriteLine($"{type} ({key.Row} / {key.Column}): OnBind");

        return Task.CompletedTask;
    }

    public override Task OnKeyUp(DeviceContext context)
    {
        Console.WriteLine($"{type} ({key.Row} / {key.Column}): OnKeyUp");

        return Task.CompletedTask;
    }

    public override Task OnKeyDown(DeviceContext context)
    {
        Console.WriteLine($"{type} ({key.Row} / {key.Column}): OnKeyDown");

        return Task.CompletedTask;
    }
}
