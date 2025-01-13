using PlainDeck.Sdk;
using PlainDeck.Sdk.Extensions;
using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Debugger;

public sealed class BlinkingKey(TimeSpan interval) : KeyHandler
{
    private PeriodicTimer? timer;

    public override Task OnBind(DeviceContext context)
    {
        timer = new PeriodicTimer(interval);
        _ = Blink(Key, context);

        return Task.CompletedTask;
    }

    private async Task Blink(DeviceKey key, DeviceContext context)
    {
        var on = true;
        while (timer is not null)
        {
            context.SetKeyImage(key, $"<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='{(on ? "red" : "green")}'/></svg>");
            on = !on;
            
            await timer.WaitForNextTickAsync();
        }
    }
}
