namespace PlainDeck.Configuration;

public sealed class StreamDeckPedal : DeviceConfiguration
{
    public override int ButtonRows => 1;
    public override int ButtonColumns => 3;

    public override bool HasStatusLed => true;
    public override bool HasSensitivity => true;
}
