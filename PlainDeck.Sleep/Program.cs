using PlainDeck.Sdk;

var device = Device.Connect();
foreach (var key in device.Keys)
{
    device.SetKey(key, "<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='black' /></svg>");
}

device.SetBrightness(0);
