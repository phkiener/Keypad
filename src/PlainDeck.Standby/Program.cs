using System.Reflection;
using PlainDeck;

var device = Device.Connect(DeviceType.StreamDeckXL2022);

var imageData = LoadImage();
//device.Wake();
//device.SetBrightness(1);
device.SetScreensaver(imageData);

static byte[] LoadImage()
{
    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PlainDeck.Standby.Image.jfif");
    
    var data = new byte[stream!.Length];
    stream.ReadExactly(data);

    return data;
}
