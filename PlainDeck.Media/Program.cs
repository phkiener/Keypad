using PlainDeck.Sdk;

var device = Device.Connect();

var playPause = device.KeyAt(0, 1);
device.SetKey(playPause, "<svg viewBox='0 0 1 1'><rect fill='white' x='0' y='0' height='1' width='1'/></svg>");
device.BindKey(playPause, ConsoleKey.MediaPlay);

device.SetBrightness(0.5);
await device.ListenAsync(CancellationToken.None);
