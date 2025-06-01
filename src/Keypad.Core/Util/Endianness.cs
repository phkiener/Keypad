namespace Keypad.Core.Util;

internal static class Endianness
{
    public static ReadOnlySpan<byte> ToLittleEndian(this short value)
    {
        byte[] data = [(byte)(value & 0xFF), (byte)((value >> 8) & 0xFF)];

        return new ReadOnlySpan<byte>(data);
    }
}
