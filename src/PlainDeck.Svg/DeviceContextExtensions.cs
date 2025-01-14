using System.Diagnostics.CodeAnalysis;
using PlainDeck.Hosting;

namespace PlainDeck.Svg;

public static class SvgExtensions
{
    public static void SetKeyImage(this IDeviceContext context, DeviceKey key, [StringSyntax("XML")] string svg)
    {
        if (!context.Device.HasKeyImage)
        {
            return;
        }
        
        var imageData = ImageConverter.SvgToJpeg(svg, context.Device.KeyImageWidth, context.Device.KeyImageHeight, context.Device.KeyImageFlip);
        context.SetKeyImage(key, imageData);
    }
}
