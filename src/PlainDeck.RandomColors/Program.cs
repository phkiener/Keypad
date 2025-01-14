using PlainDeck;
using PlainDeck.Svg;

var device = Device.Connect(DeviceType.StreamDeckXL2022);
device.SetBrightness(1);

foreach (var key in device.Keys)
{
    device.MapKey(key, new SetRandomColor());
}

await device.ListenAsync(CancellationToken.None);

internal sealed class SetRandomColor : KeyHandler
{
    public override Task OnBind(IDeviceContext context)
    {
        ApplyRandomColor(context);

        return Task.CompletedTask;
    }

    public override Task OnKeyUp(IDeviceContext context)
    {
        ApplyRandomColor(context);

        return Task.CompletedTask;
    }

    private void ApplyRandomColor(IDeviceContext device)
    {
        var randomColor = OneOf("white", "red", " blue", "green", "yellow", "orange", "grey", "purple");
        device.SetKeyImage(Key, $"<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='{randomColor}'/></svg>");
    }
    
    private static T OneOf<T>(params ReadOnlySpan<T> values)
    {
        return Random.Shared.GetItems(values, 1).Single();
    }
}
