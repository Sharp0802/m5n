using System.Runtime.InteropServices;
using M5N.Slave.Interop.Dynamics;

namespace M5N.Slave.Interop;

public partial class PythonException() : Exception(GetMessage())
{
    private static string GetMessage()
    {
        var err = PyErr_GetRaisedException();
        return ((PythonString)PyObject_Str(err)).Value;   
    }

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyErr_GetRaisedException();

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyObject_Str(IntPtr o);
}