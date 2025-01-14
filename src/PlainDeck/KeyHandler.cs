using PlainDeck.Hosting;

namespace PlainDeck;

public abstract class KeyHandler
{
    private Func<byte[]>? initialImageProvider;

    internal void SetInitialImageData(Func<byte[]> imageProvider)
    {
        initialImageProvider = imageProvider;
    }
    
    public DeviceKey Key { get; internal set; }
    public DeviceConfiguration Configuration { get; internal set; } = null!;
    
    public virtual Task OnBind(IDeviceContext context)
    {
        if (initialImageProvider is not null)
        {
            context.SetKeyImage(Key, initialImageProvider());
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
        return handler.WithImage(() => imageData);
    }
    
    public static T WithImage<T>(this T handler, Func<byte[]> imageDataProvider) where T : KeyHandler
    {
        handler.SetInitialImageData(imageDataProvider);
        return handler;
    }
}
