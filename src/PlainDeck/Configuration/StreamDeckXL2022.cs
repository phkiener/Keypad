namespace PlainDeck.Configuration;

public sealed class StreamDeckXL2022 : DeviceConfiguration
{
    public override int ButtonRows => 4;
    public override int ButtonColumns => 8;

    public override bool HasBrightness => true;
    public override bool HasKeyImage => true;

    public override int KeyImageWidth => 96;
    public override int KeyImageHeight => 96;
    public override bool KeyImageFlip => true;
    public override string KeyImageFormat => "image/jpeg";
}
