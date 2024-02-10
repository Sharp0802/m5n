using System.Runtime.InteropServices;

namespace M5N.Slave.Interop.Converters;

public abstract partial class IntegerConverter : IValueConverter<long>
{
    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyLong_FromLongLong(long v);
        
    [LibraryImport(InteropConfiguration.Python)]
    private static partial long PyLong_AsLongLong(IntPtr v);

    public static IntPtr Serialize(long cs)
    {
        return PyLong_FromLongLong(cs);
    }

    public static bool Deserialize(IntPtr py, out long result)
    {
        result = PyLong_AsLongLong(py);
        return true;
    }
}