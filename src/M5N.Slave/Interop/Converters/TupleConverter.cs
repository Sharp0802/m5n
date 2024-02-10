using System.Runtime.InteropServices;
using M5N.Slave.Interop.Dynamics;

namespace M5N.Slave.Interop.Converters;

public partial class TupleConverter : IValueConverter<PythonObject?[]>
{
    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyTuple_New(nint n);

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyTuple_SetItem(IntPtr p, nint pos, IntPtr o);

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyTuple_GetItem(IntPtr p, nint pos);
    
    [LibraryImport(InteropConfiguration.Python)]
    private static partial nint PyTuple_Size(IntPtr p);
    
    public static IntPtr Serialize(PythonObject?[] cs)
    {
        var tuple = PyTuple_New(cs.Length);
        for (var i = 0; i < cs.Length; ++i)
            PyTuple_SetItem(tuple, i, cs[i]?.Handle ?? PythonObject.None);
        return tuple;
    }

    public static bool Deserialize(IntPtr py, out PythonObject?[] result)
    {
        var size = PyTuple_Size(py);

        result = new PythonObject[size];
        for (var i = 0; i < size; ++i)
            result[i] = PythonObject.FromHandle(PyTuple_GetItem(py, i));

        return true;
    }
}