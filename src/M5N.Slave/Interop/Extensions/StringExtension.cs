using System.Text;

namespace M5N.Slave.Interop.Extensions;

public static class StringExtension
{
    public static byte[] ToCString(this string data, Encoding? enc = null)
    {
        enc ??= Encoding.UTF8;

        var bytes = new byte[enc.GetByteCount(data) + 1];
        enc.GetBytes(data, bytes);
        bytes[^1] = 0;

        return bytes;
    }
}