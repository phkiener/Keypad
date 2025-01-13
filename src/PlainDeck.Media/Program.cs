using PlainDeck.Sdk;
using PlainDeck.Sdk.Extensions;

var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellation.Cancel();

var device = Device.Connect(DeviceType.StreamDeckXL2022);
device.MapKey(0, 0, new ProxyKeyHandler(ConsoleKey.MediaPrevious).WithImage("PlainDeck.Media.Resources.Ionicons.play-skip-back-circle-sharp.svg"));
device.MapKey(0, 1, new ProxyKeyHandler(ConsoleKey.MediaPlay).WithImage("PlainDeck.Media.Resources.Ionicons.play-circle-sharp.svg"));
device.MapKey(0, 2, new ProxyKeyHandler(ConsoleKey.MediaNext).WithImage("PlainDeck.Media.Resources.Ionicons.play-skip-forward-circle-sharp.svg"));
device.MapKey(0, 5, new ProxyKeyHandler(ConsoleKey.VolumeMute).WithImage("PlainDeck.Media.Resources.Ionicons.volume-mute-sharp.svg"));
device.MapKey(0, 6, new ProxyKeyHandler(ConsoleKey.VolumeDown).WithImage("PlainDeck.Media.Resources.Ionicons.volume-low-sharp.svg"));
device.MapKey(0, 7, new ProxyKeyHandler(ConsoleKey.VolumeUp).WithImage("PlainDeck.Media.Resources.Ionicons.volume-high-sharp.svg"));
device.MapKey(3, 7, new ProxyKeyHandler(ConsoleKey.Escape).WithImage("PlainDeck.Media.Resources.Ionicons.exit-sharp.svg"));

device.SetBrightness(0.5);
await device.ListenAsync(cancellation.Token);

device.SetBrightness(0);
