using System.Diagnostics.CodeAnalysis;
using System.Xml;
using PlainDeck.Sdk.Hosting;
using SkiaSharp;
using Svg.Skia;

namespace PlainDeck.Sdk.Extensions;

public static class SvgExtensions
{
    public static void SetKeyImage(this DeviceContext context, DeviceKey key, [StringSyntax("XML")] string svg)
    {
        if (!context.Device.HasKeyImage)
        {
            return;
        }
        
        var imageData = SvgToJpeg(svg, context.Device.KeyImageWidth, context.Device.KeyImageHeight, context.Device.KeyImageFlip);
        context.SetKeyImage(key, imageData);
    }
    
    private static byte[] SvgToJpeg(string svg, int width, int height, bool doFlip)
    {
        using var bitmap = GetBitmap(svg, width, height);
        using var flippedBitmap = doFlip ? Flip(bitmap) : bitmap;

        return flippedBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
    }

    private static SKBitmap GetBitmap(string svg, int width, int height)
    {
        var xml = new XmlDocument();
        xml.LoadXml(svg);
        xml.DocumentElement!.SetAttribute("width", width.ToString());
        xml.DocumentElement!.SetAttribute("height", height.ToString());
        
        var svgCanvas = SKSvg.CreateFromSvg(xml.OuterXml);
        if (svgCanvas.Picture is null)
        {
            throw new InvalidOperationException("Cannot create bitmap from SVG");
        }
        
        var bitmap = svgCanvas.Picture.ToBitmap(SKColors.Transparent, 1, 1, SKColorType.Rgba8888, SKAlphaType.Premul, SKColorSpace.CreateSrgb());
        return bitmap ?? throw new InvalidOperationException("Cannot create bitmap from SVG");
    }

    private static SKBitmap Flip(SKBitmap original)
    {
        var flipped = new SKBitmap(original.Width, original.Height, original.Info.ColorType, original.Info.AlphaType);

        using var canvas = new SKCanvas(flipped);
        canvas.Translate(original.Width, 0);
        canvas.Scale(-1, 1);
        canvas.DrawBitmap(original, 0, 0);

        return flipped;
    }
}
