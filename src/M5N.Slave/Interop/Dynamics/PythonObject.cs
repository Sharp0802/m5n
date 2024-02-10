using System.Runtime.InteropServices;

namespace M5N.Slave.Interop.Dynamics;

public abstract partial class PythonObject : IPythonObject
{
    [DllImport(InteropConfiguration.FnExports)]
    private static extern IntPtr GetAddressOfNoneObject();

    public static IntPtr None => GetAddressOfNoneObject();
    
    public abstract Type   Type   { get; }
    public abstract IntPtr Handle { get; }
}