namespace Keypad.Cli.ArgParse;

public sealed class ArgumentParser(IReadOnlyList<IBinder> binders)
{
    public ArgumentParser WithBinder(params IEnumerable<IBinder> additionalBinders)
    {
        return new ArgumentParser([..binders, ..additionalBinders]);
    }
    
    public T Parse<T>(string[] args) => (T)Parse(typeof(T), args);

    private object Parse(Type optionsType, string[] args)
    {
        var switches = Introspection.FindSwitches(optionsType);
        var options = Introspection.FindOptions(optionsType);
        var arguments = Introspection.FindArguments(optionsType);
        var subcommands = Introspection.FindSubcommands(optionsType);

        var initializers = new List<Action<object>>();
        var constructorArguments = new List<object?>();

        Option? currentOption = null;
        bool inArguments = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (arg is "--")
            {
                inArguments = true;
                continue;
            }

            if (!inArguments)
            {
                if (currentOption is not null)
                {
                    if (!TryConvert(arg, currentOption.Type, out object? optionValue))
                    {
                        throw new InvalidOperationException($"Cannot parse {arg} as {currentOption.Type} for option {currentOption.Property.Name}");
                    }

                    var option = currentOption;
                    initializers.Add(obj => option.Property.SetValue(obj, optionValue));
                    currentOption = null;

                    continue;
                }

                if (arg.StartsWith("--"))
                {
                    var matchingSwitch = switches.FirstOrDefault(s => s.Name == arg[2..]);
                    if (matchingSwitch is not null)
                    {
                        initializers.Add(obj => matchingSwitch.Property.SetValue(obj, true));
                        continue;
                    }

                    var matchingOption = options.FirstOrDefault(o => o.Name == arg[2..]);
                    if (matchingOption is not null)
                    {
                        currentOption = matchingOption;
                        continue;
                    }
                }
            }

            inArguments = true;

            var matchingSubcommand = subcommands.FirstOrDefault(sc => sc.Name == arg);
            if (matchingSubcommand is not null)
            {
                var subcommand = Parse(matchingSubcommand.Type, args[(i + 1)..]);
                foreach (var initializer in initializers)
                {
                    initializer.Invoke(subcommand);
                }

                return subcommand;
            }

            var currentArgument = arguments[constructorArguments.Count];
            if (!TryConvert(arg, currentArgument.Type, out object? argumentValue))
            {
                throw new InvalidOperationException($"Cannot parse {arg} as {currentArgument.Type} for argument {constructorArguments.Count + 1}");
            }

            constructorArguments.Add(argumentValue);
        }

        var instance = Activator.CreateInstance(optionsType, constructorArguments.ToArray())!;
        foreach (var initializer in initializers)
        {
            initializer.Invoke(instance);
        }

        return instance;
    }

    private bool TryConvert(string argument, Type targetType, out object? value)
    {
        foreach (var binder in binders.Where(b => b.CanBind(targetType)).ToList())
        {
            if (binder.TryBind(argument, targetType, out value))
            {
                return true;
            }
        }

        value = null;
        return false;
    }
}
