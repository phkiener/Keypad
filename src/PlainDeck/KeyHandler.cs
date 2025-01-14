using PlainDeck.Hosting;

namespace PlainDeck;

public abstract class KeyHandler
{
    private byte[]? initialImageData;

    internal void SetInitialImageData(byte[] imageData)
    {
        initialImageData = imageData;
    }
    
    public DeviceKey Key { get; internal set; }
    public DeviceConfiguration Configuration { get; internal set; } = null!;
    
    public virtual Task OnBind(IDeviceContext context)
    {
        if (initialImageData is not null)
        {
            context.SetKeyImage(Key, initialImageData);
        }

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

public static class KeyHandlerExtensions
{
    public static T WithImage<T>(this T handler, byte[] imageData) where T : KeyHandler
    {
        handler.SetInitialImageData(imageData);
        return handler;
    }
}
