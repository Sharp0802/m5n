using System.Data;
using System.Runtime.InteropServices;

namespace M5N.Interop.Python;

public sealed class PythonInteropEngine : IInteropEngine
{
    private static readonly object Lock = new();
    private static          bool   _init;
    
    public PythonInteropEngine(string directory)
    {
        lock (Lock)
        {
            if (_init)
                throw new DuplicateNameException();
            _init = true;
            
            PyMarshal.Initialize();

            var r = PyMarshal.RunSimpleString($"import sys;sys.path.append('{directory}')");
            if (r != 0)
                throw new ExternalException("Couldn't execute python code.", r);
        }
    }

    public string Name { get; } = "cpython 3.11";
    
    public dynamic ImportModule(string name)
    {
        ObjectDisposedException.ThrowIf(!_init, this);
        return new PythonInteropModule(name);
    }

    public void Dispose()
    {
        lock (Lock)
        {
            if (_init)
                PyMarshal.Uninitialize();
            _init = false;
        }
    }
}
