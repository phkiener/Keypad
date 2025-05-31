using System.Collections;
using System.Text.Json;

namespace Keypad.Configuration;

public static class ConfigurationLoader
{
    public static string FindConfigurationFile(IDictionary environment)
    {
        if (environment["KEYPAD_CONFIGPATH"] is string directPath)
        {
            return directPath;
        }

        var configDirectory = environment["XDG_CONFIG_HOME"] as string
                              ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

        return Path.Combine(configDirectory, "keypad", "config.json");
    }
    
    public static async Task<KeypadConfig> LoadConfigurationAsync(Stream configStream)
    {
        var parsedConfiguration = await JsonSerializer.DeserializeAsync<KeypadConfig>(configStream);

        return parsedConfiguration ?? throw new FormatException("Cannot parse configuration.");
    }
}

