using Keypad;
using PlainDeck;
using YamlDotNet.Serialization;
using KeyHandler = Keypad.KeyHandler;

var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

var (debug, configFile, workingDirectory) = ParseArguments(args);
var configuration = ReadConfiguration(configFile, debug);

var streamDeckXl2022 = StartDeviceHost(DeviceType.StreamDeckXL2022, configuration.StreamDeckXL2022, workingDirectory, cancellationTokenSource.Token);
var streamDeckPedal = StartDeviceHost(DeviceType.StreamDeckPedal, configuration.StreamDeckPedal, workingDirectory, cancellationTokenSource.Token);

await Task.WhenAll(streamDeckXl2022, streamDeckPedal);
return 0;

static (bool Debug, string Path, string WorkingDirectory) ParseArguments(string[] args)
{
    var debug = args[0] is "--debug";
    var path = debug ? args[1] : args[0];
    var directory = Path.GetDirectoryName(path)!;

    return (debug, path, directory);
}

static Configuration ReadConfiguration(string path, bool debug)
{
    if (debug)
    {
        Console.WriteLine("Reading configuration from {0}", path);
    }

    using var stream = File.OpenRead(path);
    using var reader = new StreamReader(stream);

    var serializer = new DeserializerBuilder().Build();
    return serializer.Deserialize<Configuration>(reader);
}

static Task StartDeviceHost(DeviceType type, KeyConfiguration[] keys, string directory, CancellationToken cancellationToken)
{
    if (keys is [])
    {
        return Task.CompletedTask;
    }

    var host = Device.Connect(type);
    foreach (var key in keys)
    {
        var keyHandler = new KeyHandler(key.Key, key.Icon is null ? null : Path.Combine(directory, key.Icon));
        host.MapKey(key.Row, key.Column, keyHandler);
    }

    host.SetBrightness(.8);
    return host.ListenAsync(cancellationToken);
}
