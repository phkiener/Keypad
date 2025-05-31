using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Keypad.Tests.Configuration;

public sealed partial class ConfigurationLoaderTest
{
    private static MemoryStream BuildConfigFile([StringSyntax(StringSyntaxAttribute.Json)] string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
