using PlainDeck;

var state = new State { Sensitivity = 0.5 };

var device = Device.Connect(DeviceType.StreamDeckPedal);
device.SetSensitivity(state.Sensitivity);
device.SetStatusLed(0, 0, 0);

device.MapKey(0, 0, new ChangeSensitivity(state, static d => d - 0.1));
device.MapKey(0, 2, new ChangeSensitivity(state, static d => d + 0.1));
device.MapKey(0, 1, new BlinkLed());

await device.ListenAsync(CancellationToken.None);

internal sealed class State
{
    public double Sensitivity { get; set; } = 0.35;
}

internal sealed class BlinkLed : KeyHandler
{
    public override Task OnKeyDown(IDeviceContext context)
    {
        context.SetStatusLed(255, 255, 255);
        Console.WriteLine("Blink!");
        
        return Task.CompletedTask;
    }

    public override Task OnKeyUp(IDeviceContext context)
    {
        context.SetStatusLed(0, 0, 0);
        
        return Task.CompletedTask;
    }
}

internal sealed class ChangeSensitivity(State state, Func<double, double> change) : KeyHandler
{
    public override Task OnKeyDown(IDeviceContext context)
    {
        var adjusted = Math.Clamp(change(state.Sensitivity), 0.0, 0.7);

        state.Sensitivity = adjusted;
        context.SetSensitivity(adjusted);
        
        Console.WriteLine($"Sensitivity is {adjusted:F1}");

        return Task.CompletedTask;
    }
}
