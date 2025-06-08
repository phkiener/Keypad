using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Keypad.Core.Abstractions;
using SkiaSharp;
using Svg.Skia;

namespace Keypad.Core.Image;

internal static class ImageLoader
{
    public static SKBitmap Load(DeviceImage image, int width, int height)
    {
        return image switch
        {
            DeviceImage.File { Path: var filePath } => LoadImage(filePath, width, height),
            DeviceImage.Color { Value: var color } => LoadSvg($"<svg viewBox='0 0 1 1'><path d='M0 0h1v1H0z' fill='{color}'/></svg>", width, height),
            _ => throw new NotSupportedException($"DeviceImage {image} is not supported.")
        };
    }

    public static SKBitmap Flip(this SKBitmap bitmap, bool horizontal, bool vertical)
    {
        var flippedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        using var canvas = new SKCanvas(flippedBitmap);

        canvas.Translate(horizontal ? bitmap.Width : 0, vertical ? bitmap.Height : 0);
        canvas.Scale(horizontal ? -1 : 1, vertical ? -1 : 1);
        canvas.DrawBitmap(bitmap, 0, 0);

        return flippedBitmap;
    }
    
    private static SKBitmap LoadImage(string path, int width, int height)
    {
        using var image = SKImage.FromEncodedData(path);
        using var originalBitmap = SKBitmap.FromImage(image);

        var scaledBitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(scaledBitmap);
        canvas.Scale(width / (float)originalBitmap.Width, height / (float)originalBitmap.Height);
        canvas.DrawBitmap(originalBitmap, 0, 0);

        return scaledBitmap;
    }
    
    private static SKBitmap LoadSvg([StringSyntax(StringSyntaxAttribute.Xml)] string content, int width, int height)
    {
        var xml = new XmlDocument();
        xml.LoadXml(content);
        xml.DocumentElement!.SetAttribute("width", width.ToString());
        xml.DocumentElement!.SetAttribute("height", height.ToString());
        
        using var svgCanvas = SKSvg.CreateFromSvg(xml.OuterXml);
        return svgCanvas.Picture?.ToBitmap(SKColors.Transparent, 1, 1, SKColorType.Rgba8888, SKAlphaType.Premul, SKColorSpace.CreateSrgb())
               ?? throw new InvalidOperationException("Failed to load image from given SVG.");
    }
}
