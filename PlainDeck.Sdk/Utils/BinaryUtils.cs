namespace PlainDeck.Sdk.Utils;

internal static class BinaryUtils
{
    public static byte[] ToLittleEndian(this int value) => [(byte)(value & 0xFF), (byte)((value >> 8) & 0xFF)];
}
