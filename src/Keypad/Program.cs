using Keypad.Configuration;

var configPath = ConfigurationLoader.FindConfigurationFile(Environment.GetEnvironmentVariables());
await using var configStream = File.OpenRead(configPath);
var config = await ConfigurationLoader.LoadConfigurationAsync(configStream);

return 0;
