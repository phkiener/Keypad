using System.Reflection;
using System.Text;

namespace Keypad.Cli.ArgParse;

internal sealed record Switch(string Name, char? ShortName, PropertyInfo Property);

internal sealed record Option(string Name, char? ShortName, Type Type, PropertyInfo Property);

internal sealed record Argument(Type Type);

internal sealed record Subcommand(string Name, Type Type);

internal static class Introspection
{
    public static Switch[] FindSwitches(Type type)
    {
        return type.GetProperties()
            .Where(static p => p.PropertyType == typeof(bool) || p.PropertyType == typeof(bool?))
            .Select(BuildSwitch)
            .ToArray();
    }

    private static Switch BuildSwitch(PropertyInfo property)
    {
        var longName = ToKebapCase(property.Name);
        var shortName = HasUniqueFirstLetter(property) ? longName[0] : null as char?;

        return new Switch(longName, shortName, property);
    }

    public static Option[] FindOptions(Type type)
    {
        return type.GetProperties()
            .Where(static p => p.PropertyType != typeof(bool) && p.PropertyType != typeof(bool?))
            .Select(BuildOption)
            .ToArray();
    }

    private static Option BuildOption(PropertyInfo property)
    {
        var longName = ToKebapCase(property.Name);
        var shortName = HasUniqueFirstLetter(property) ? longName[0] : null as char?;

        return new Option(longName, shortName, property.PropertyType, property);
    }

    public static Argument[] FindArguments(Type type)
    {
        var constructor = type.GetConstructors().SingleOrDefault();
        if (constructor is null)
        {
            return [];
        }

        return constructor.GetParameters().Select(static p => new Argument(p.ParameterType)).ToArray();
    }

    public static Subcommand[] FindSubcommands(Type type)
    {
        return type.GetNestedTypes().Select(static t => new Subcommand(ToKebapCase(t.Name), t)).ToArray();
    }

    private static string ToKebapCase(string pascalCase)
    {
        var name = new StringBuilder();

        foreach (var chararacter in pascalCase)
        {
            if (char.IsUpper(chararacter) && name.Length > 0)
            {
                name.Append('-');
            }

            name.Append(char.ToLower(chararacter));
        }

        return name.ToString();
    }

    private static bool HasUniqueFirstLetter(PropertyInfo property)
    {
        if (property.DeclaringType is null)
        {
            return false;
        }

        var firstLetter = property.Name[0];
        return property.DeclaringType?.GetProperties().Count(p => p.Name.StartsWith(firstLetter)) is 1;
    }
}
