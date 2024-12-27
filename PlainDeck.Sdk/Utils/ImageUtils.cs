using System.Xml;
using SkiaSharp;
using Svg.Skia;

namespace PlainDeck.Sdk.Utils;

internal static class ImageUtils
{
    public static byte[] SvgToJpeg(string svg, int width, int height)
    {
        var xml = new XmlDocument();
        xml.LoadXml(svg);
        xml.DocumentElement!.SetAttribute("width", width.ToString());
        xml.DocumentElement!.SetAttribute("height", height.ToString());
        
        var svgCanvas = SKSvg.CreateFromSvg(xml.OuterXml);
        using var memoryStream = new MemoryStream();
        svgCanvas.Save(memoryStream, SKColors.Transparent, SKEncodedImageFormat.Jpeg);

        return memoryStream.ToArray();
    }
}
