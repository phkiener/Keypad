using System.Reflection;
using PlainDeck;
using PlainDeck.Svg;
using SkiaSharp;

var device = Device.Connect(DeviceType.StreamDeckXL2022);

var imageData = LoadImage();

foreach (var key in device.Keys)
{
    device.SetKeyImage(key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='black' /></svg>");
}

device.SetStandbyImage(imageData);
device.SetTimeout(10);
var listener = device.ListenAsync();

await listener;

static byte[] LoadImage()
{
    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PlainDeck.Standby.Image.jpg");
    
    var data = new byte[stream!.Length];
    stream.ReadExactly(data);

    // We need a horizontal AND a vertical flip for this image.
    // What.
    using var image = SKImage.FromEncodedData(data);
    using var bitmap = SKBitmap.FromImage(image);
    using var canvas = new SKCanvas(bitmap);
    canvas.Translate(bitmap.Width, bitmap.Height);
    canvas.Scale(-1, -1);
    canvas.DrawBitmap(bitmap, 0, 0);
    
    
    return bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
}
