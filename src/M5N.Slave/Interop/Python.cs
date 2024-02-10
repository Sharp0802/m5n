using System.Runtime.InteropServices;
using M5N.Slave.Interop.Extensions;

namespace M5N.Slave.Interop;


public sealed partial class Python : IDisposable
{
    private static int _initialized;
    
    public Python(string directory)
    {
        if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 1)
            throw new InvalidOperationException();
        
        Py_Initialize();

        unsafe
        {
            int r;
            fixed (byte* src = $"import sys;sys.path.append('{directory}')".ToCString())
                r = PyRun_SimpleString((IntPtr)src);
            if (r != 0)
                throw new ExternalException("Couldn't run code", r);
        }
    }

    [LibraryImport(InteropConfiguration.Python)]
    private static partial void Py_Initialize();

    [LibraryImport(InteropConfiguration.Python)]
    private static partial int Py_FinalizeEx();
    
    [LibraryImport(InteropConfiguration.Python)]
    private static partial int PyRun_SimpleString(IntPtr name);

    public Module Import(string name)
    {
        if (_initialized == 0)
            throw new TypeInitializationException(typeof(Python).FullName, null);
        return new Module(name);
    }

    public void Dispose()
    {
        Py_FinalizeEx();
        _initialized = 0;
    }
}