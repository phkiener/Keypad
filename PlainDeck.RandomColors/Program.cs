using PlainDeck.Sdk;
using PlainDeck.Sdk.Model;

var device = Device.Connect();
device.SetBrightness(1);

foreach (var key in device.Keys)
{
    SetRandomColor(device, key);
}

device.OnKeyUp += (_, key) => SetRandomColor(device, key);
device.OnKeyDown += (_, key) => SetRandomColor(device, key);

await device.ListenAsync(CancellationToken.None);
return;

static void SetRandomColor(Device device, DeviceKey key)
{
    var randomColor = OneOf("white", "red", " blue", "green", "yellow", "orange", "grey", "purple");
    device.SetKey(key, $"<svg viewBox='0 0 1 1'><rect x='0' y='0' width='1' height='1' fill='{randomColor}'/></svg>");
}

static T OneOf<T>(params ReadOnlySpan<T> values)
{
    return Random.Shared.GetItems(values, 1).Single();
}
