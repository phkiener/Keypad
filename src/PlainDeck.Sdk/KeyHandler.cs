namespace PlainDeck.Sdk;

public abstract class KeyHandler
{
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
