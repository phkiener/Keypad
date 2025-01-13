using System.Reflection;

namespace PlainDeck.Sdk.Extensions;

public partial class ProxyKeyHandler(ConsoleKey key) : KeyHandler
{
    private string? imageSvg;

    public ProxyKeyHandler WithImage(string resourcePath)
    {
        var resourceAssembly = Assembly.GetCallingAssembly();
        using var embeddedResource = resourceAssembly.GetManifestResourceStream(resourcePath);
        if (embeddedResource is null)
        {
            throw new InvalidOperationException($"Embedded resource \"{resourcePath}\" could not be found.");
        }
        
        using var reader = new StreamReader(embeddedResource);
        imageSvg = reader.ReadToEnd();

        return this;
    }

    public override Task OnBind(IDeviceContext context)
    {
        if (imageSvg is not null)
        {
            context.SetKeyImage(Key, imageSvg);
        }

        return Task.CompletedTask;
    }

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
