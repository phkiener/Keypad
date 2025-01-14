using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using PlainDeck.Sdk;

namespace PlainDeck.Svg;

public static class KeyHandlerExtensions
{
    public static T WithImage<T>(this T handler, [StringSyntax("XML")] string svg) where T : KeyHandler
    {
        var imageData = ImageConverter.SvgToJpeg(
            svg: svg,
            width: handler.Configuration.KeyImageWidth,
            height: handler.Configuration.KeyImageHeight,
            doFlip: handler.Configuration.KeyImageFlip);

        return handler.WithImage(imageData);
    }
    
    public static T WithSvgFrom<T>(this T handler, string resourcePath) where T : KeyHandler
    {
        var resourceAssembly = Assembly.GetCallingAssembly();
        using var embeddedResource = resourceAssembly.GetManifestResourceStream(resourcePath);
        if (embeddedResource is null)
        {
            throw new InvalidOperationException($"Embedded resource \"{resourcePath}\" could not be found.");
        }
        
        using var reader = new StreamReader(embeddedResource);
        var svg = reader.ReadToEnd();

        return handler.WithImage(svg);
    }
}
