namespace PlainDeck.Host;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var cancelSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, _) => cancelSource.Cancel();

        var device = DeviceHandle.Find();
        await device.ListenAsync(cancelSource.Token);
    }
}
