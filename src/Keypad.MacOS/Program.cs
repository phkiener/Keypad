using Keypad.Configuration;
using Keypad.MacOS;

var config = new KeypadConfig { Devices = [] };
var configPath = ConfigurationLoader.FindConfigurationFile(Environment.GetEnvironmentVariables());
if (File.Exists(configPath))
{
    await using var readStream = File.OpenRead(configPath);
    config = await ConfigurationLoader.LoadConfigurationAsync(readStream);
}

NSApplication.Init();
NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Accessory;
NSApplication.SharedApplication.Delegate = new AppDelegate(config);
NSApplication.SharedApplication.Run();

return 0;
