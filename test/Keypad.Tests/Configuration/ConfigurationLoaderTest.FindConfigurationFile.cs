using Keypad.Configuration;

namespace Keypad.Tests.Configuration;

public sealed partial class ConfigurationLoaderTest
{
    [Test]
    public async Task UsesDefaultLocation_IfNoConfigurationIsSet()
    {
        var environment = new Dictionary<string, string>();
        var chosenPath = ConfigurationLoader.FindConfigurationFile(environment);

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        await Assert.That(chosenPath).IsEqualTo(Path.Combine(userProfilePath, ".config", "keypad", "config.json"));
    }

    [Test]
    public async Task UsesXDGConfigHomePath()
    {
        var environment = new Dictionary<string, string> { ["XDG_CONFIG_HOME"] = "/somewhere" };

        var chosenPath = ConfigurationLoader.FindConfigurationFile(environment);
        await Assert.That(chosenPath).IsEqualTo(Path.Combine("/somewhere", "keypad", "config.json"));
    }

    [Test]
    public async Task UsesExplicitlyGivenPath()
    {
        var environment = new Dictionary<string, string> { ["KEYPAD_CONFIGPATH"] = "/somewhere/config.json" };

        var chosenPath = ConfigurationLoader.FindConfigurationFile(environment);
        await Assert.That(chosenPath).IsEqualTo("/somewhere/config.json");
    }
}