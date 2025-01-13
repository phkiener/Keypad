using PlainDeck.Sdk.Hosting;

namespace PlainDeck.Sdk;

public abstract class KeyHandler
{
    public DeviceKey Key { get; internal set; }
    
    public virtual Task OnBind(IDeviceContext context)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task OnKeyDown(IDeviceContext context)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task OnKeyUp(IDeviceContext context)
    {
        return Task.CompletedTask;
    }
}
