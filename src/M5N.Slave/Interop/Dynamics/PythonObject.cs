using System.Runtime.InteropServices;

namespace M5N.Slave.Interop.Dynamics;

public abstract partial class PythonObject : IPythonObject
{
    [DllImport(InteropConfiguration.FnExports)]
    public static extern IntPtr GetAddressOfNoneObject();
    
    public abstract Type   Type   { get; }
    public abstract IntPtr Handle { get; }
}