using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace M5N.Slave.Interop.Converters;

public abstract partial class StringConverter : IValueConverter<string>
{
    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyUnicode_FromStringAndSize(IntPtr v, nint len);

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyUnicode_AsUTF8(IntPtr v);

    public static IntPtr Serialize(string cs)
    {
        unsafe
        {
            var bytes = Encoding.UTF8.GetBytes(cs);
            fixed (byte* p = bytes)
                return PyUnicode_FromStringAndSize((IntPtr)p, bytes.Length);
        }
    }

    public static bool Deserialize(IntPtr py, [NotNullWhen(true)] out string? result)
    {
        var lpsz = PyUnicode_AsUTF8(py);
        result = Marshal.PtrToStringUTF8(lpsz);
        return result is not null;
    }
}