using PlainDeck.Sdk;

var device = Device.Connect();

var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellation.Cancel();

device.ConfigureKey(row: 0, column: 0)
    .BindKey(ConsoleKey.MediaPrevious)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.play-skip-back-circle-sharp.svg");

device.ConfigureKey(row: 0, column: 1)
    .BindKey(ConsoleKey.MediaPlay)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.play-circle-sharp.svg");

device.ConfigureKey(row: 0, column: 2)
    .BindKey(ConsoleKey.MediaNext)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.play-skip-forward-circle-sharp.svg");

device.ConfigureKey(row: 0, column: 5)
    .BindKey(ConsoleKey.VolumeMute)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.volume-mute-sharp.svg");

device.ConfigureKey(row: 0, column: 6)
    .BindKey(ConsoleKey.VolumeDown)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.volume-low-sharp.svg");

device.ConfigureKey(row: 0, column: 7)
    .BindKey(ConsoleKey.VolumeUp)
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.volume-high-sharp.svg");

device.ConfigureKey(row: 3, column: 7)
    .BindKey(() => cancellation.Cancel())
    .SetIconFromResource("PlainDeck.Media.Resources.Ionicons.exit-sharp.svg");

device.SetBrightness(0.5);
await device.ListenAsync(cancellation.Token);

device.SetBrightness(0);
