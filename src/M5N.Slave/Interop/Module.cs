using System.Runtime.InteropServices;
using M5N.Slave.Interop.Dynamics;
using M5N.Slave.Interop.Extensions;

namespace M5N.Slave.Interop;

public partial class Module
{
    private readonly IntPtr _handle;

    public Module(string name)
    {
        unsafe
        {
            fixed (byte* p = name.ToCString())
                _handle = PyImport_ImportModule((IntPtr)p);
            if (_handle == 0)
                throw new PythonException();
        }
    }

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyImport_ImportModule(IntPtr name);

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyObject_GetAttrString(IntPtr o, IntPtr name);

    [LibraryImport(InteropConfiguration.Python)]
    private static partial IntPtr PyObject_CallObject(IntPtr callable, IntPtr args);


    public delegate PythonObject? PyFunctionDelegate(params PythonObject[] args);

    public PyFunctionDelegate? GetMethod(string name)
    {
        unsafe
        {
            IntPtr fp;
            fixed (byte* p = name.ToCString()) 
                fp = PyObject_GetAttrString(_handle, (IntPtr)p);
            if (fp == IntPtr.Zero)
                return null;
            return args => PythonObject.FromHandle(Invoke(fp, args));
        }

        static IntPtr Invoke(IntPtr func, PythonObject[] args)
        {
            return PyObject_CallObject(func, ((PythonTuple)args).Handle);
        }
    }
}