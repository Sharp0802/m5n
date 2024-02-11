using System.Runtime.InteropServices;

namespace M5N.Interop.NativeLibrary;

public sealed class CInteropEngine : IInteropEngine
{
    public string Name => $"CLR {RuntimeEnvironment.GetSystemVersion()}";
    
    public dynamic ImportModule(string name)
    {
        return new CInteropModule(name);
    }

    public void Dispose()
    {
    }
}