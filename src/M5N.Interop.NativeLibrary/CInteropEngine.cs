using System.Runtime.InteropServices;

namespace M5N.Interop.NativeLibrary;

public sealed class CInteropEngine : IInteropEngine
{
    private readonly Dictionary<string, CInteropModule> _modules = new();
    
    public string Name => $"CLR {RuntimeEnvironment.GetSystemVersion()}";

    public void ConfigureModule(string name, Action<CInteropModule> config)
    {
        if (!_modules.TryGetValue(name, out var module))
        {
            module = new CInteropModule(System.Runtime.InteropServices.NativeLibrary.Load(name));
            _modules.Add(name, module);
        }

        config(module);
    }
    
    public dynamic ImportModule(string name)
    {
        if (!_modules.TryGetValue(name, out var module))
            throw new KeyNotFoundException($"Module '{name}' not configured");

        return module;
    }
    
    public void Dispose()
    {
        foreach (var module in _modules.Values)
            module.Dispose();
    }
}