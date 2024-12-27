using System.Drawing.Imaging;
using Svg;

namespace PlainDeck.Sdk.Utils;

internal static class ImageUtils
{
    public static byte[] SvgToJpeg(string svg, int width, int height)
    {
        var svgDocument = SvgDocument.FromSvg<SvgDocument>(svg);
        var bitmap = svgDocument.Draw(rasterWidth: width, rasterHeight: height);

        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Jpeg);

        return memoryStream.ToArray();
    }
}
