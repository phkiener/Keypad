using PlainDeck.Sdk;
using PlainDeck.Sdk.Extensions;
using PlainDeck.Sdk.Svg;

var device = Device.Connect(DeviceType.StreamDeckXL2022);
foreach (var key in device.Keys)
{
    device.SetKeyImage(key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='black' /></svg>");
}

device.SetBrightness(0);
