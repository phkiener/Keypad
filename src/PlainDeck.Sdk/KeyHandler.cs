using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Sdk;

public abstract class KeyHandler
{
    public DeviceKey Key { get; internal set; }
    
    public virtual Task OnBind(DeviceContext context)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task OnKeyDown(DeviceContext context)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task OnKeyUp(DeviceContext context)
    {
        return Task.CompletedTask;
    }
}
