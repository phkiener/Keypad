using Keypad;
using PlainDeck;
using Swallow.Console.Arguments;
using KeyHandler = Keypad.KeyHandler;

var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

var options = ArgParse.Parse<Options>(args);
var configuration = Configuration.ReadFrom(options.ConfigFile);

var streamDeckXl2022 = StartDeviceHost(DeviceType.StreamDeckXL2022, configuration.StreamDeckXL2022, cancellationTokenSource.Token);
var streamDeckPedal = StartDeviceHost(DeviceType.StreamDeckPedal, configuration.StreamDeckPedal, cancellationTokenSource.Token);

await Task.WhenAll(streamDeckXl2022, streamDeckPedal);
return 0;

static Task StartDeviceHost(DeviceType type, KeyConfiguration[] keys, CancellationToken cancellationToken)
{
    if (keys is [])
    {
        return Task.CompletedTask;
    }

    var host = Device.Connect(type);
    foreach (var key in keys)
    {
        KeyHandler.Map(host, key);
    }

    host.SetBrightness(.8);
    return host.ListenAsync(cancellationToken);
}
