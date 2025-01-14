using PlainDeck;
using PlainDeck.Extensions;
using PlainDeck.Svg;

var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellation.Cancel();

var device = Device.Connect(DeviceType.StreamDeckXL2022);
device.MapKey(0, 0, new ProxyKeyHandler(ConsoleKey.MediaPrevious).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.play-skip-back-circle-sharp.svg"));
device.MapKey(0, 1, new ProxyKeyHandler(ConsoleKey.MediaPlay).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.play-circle-sharp.svg"));
device.MapKey(0, 2, new ProxyKeyHandler(ConsoleKey.MediaNext).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.play-skip-forward-circle-sharp.svg"));
device.MapKey(0, 5, new ProxyKeyHandler(ConsoleKey.VolumeMute).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.volume-mute-sharp.svg"));
device.MapKey(0, 6, new ProxyKeyHandler(ConsoleKey.VolumeDown).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.volume-low-sharp.svg"));
device.MapKey(0, 7, new ProxyKeyHandler(ConsoleKey.VolumeUp).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.volume-high-sharp.svg"));
device.MapKey(3, 7, new ProxyKeyHandler(ConsoleKey.A).WithSvgFrom("PlainDeck.Media.Resources.Ionicons.exit-sharp.svg"));

device.SetBrightness(0.5);
await device.ListenAsync(cancellation.Token);

device.SetBrightness(0);
