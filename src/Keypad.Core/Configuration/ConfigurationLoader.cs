using System.Collections;
using System.Text.Json;

namespace Keypad.Configuration;

/// <summary>
/// Helper to work with the <see cref="KeypadConfig"/>
/// </summary>
public static class ConfigurationLoader
{
    /// <summary>
    /// Find the path to the configuration file
    /// </summary>
    /// <param name="environment">The environment to use; usually taken from <see cref="Environment"/></param>
    /// <returns>Path to the configuration file</returns>
    public static string FindConfigurationFile(IDictionary environment)
    {
        if (environment["KEYPAD_CONFIGPATH"] is string directPath)
        {
            return directPath;
        }

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configDirectory = environment["XDG_CONFIG_HOME"] as string
                              ?? Path.Combine(userProfilePath, ".config");

        return Path.Combine(configDirectory, "keypad", "config.json");
    }
    
    /// <summary>
    /// Load the configuration from the given stream
    /// </summary>
    /// <param name="configStream">The stream to read the config from</param>
    /// <returns>Fully parsed <see cref="KeypadConfig"/></returns>
    /// <exception cref="FormatException">If the config cannot be parsed</exception>
    /// <exception cref="NotSupportedException">If the given stream is not readable</exception>
    public static async Task<KeypadConfig> LoadConfigurationAsync(Stream configStream)
    {
        if (!configStream.CanRead)
        {
            throw new NotSupportedException("Cannot read from the given stream.");
        }
        
        var parsedConfiguration = await JsonSerializer.DeserializeAsync<KeypadConfig>(configStream);

        return parsedConfiguration ?? throw new FormatException("Cannot parse configuration.");
    }
}

