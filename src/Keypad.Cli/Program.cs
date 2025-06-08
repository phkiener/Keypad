using Keypad.Cli;
using Keypad.Cli.ArgParse;
using Keypad.Core;

var command = Parser.DefaultParser
    .WithBinder(new DeviceTypeBinder())
    .Parse<Command>(args);

if (command.DeviceType is not null || command.SerialNumber is not null)
{
    var device = DeviceManager.Connect(deviceType: command.DeviceType, serialNumber: command.SerialNumber);
    return device is null ? 1 : command.Invoke(device);
}

foreach (var serialNumber in DeviceManager.EnumerateDevices())
{
    var device = DeviceManager.Connect(deviceType: null, serialNumber: serialNumber);
    if (device is not null)
    {
        command.Invoke(device);
    }
}

return 0;

