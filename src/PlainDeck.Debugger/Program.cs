using PlainDeck.Debugger;
using PlainDeck.Sdk;

var pedal = StartListening(DeviceType.StreamDeckPedal);
var deck = StartListening(DeviceType.StreamDeckXL2022);

await Task.WhenAll(pedal, deck);
return 0;

static async Task StartListening(DeviceType type)
{
    var device = Device.Connect(type);
    foreach (var key in device.Keys)
    {
        device.MapKey(key, new DebuggerKey(type));
    }
    
    device.MapKey(0, 0, new BlinkingKey(TimeSpan.FromMilliseconds(250)));

    await device.ListenAsync(CancellationToken.None);
}
