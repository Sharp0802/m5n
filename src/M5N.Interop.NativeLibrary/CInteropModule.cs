using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace M5N.Interop.NativeLibrary;

public sealed class CInteropModule : InteropModule, IDisposable
{
    private readonly Dictionary<string, Delegate>                          _delegates  = new();
    private readonly Dictionary<string, (Type Signature, Type[] TypeArgs)> _signatures = new();
    private readonly IntPtr                                                _module;

    public CInteropModule(IntPtr module)
    {
        _module = module;
    }
    
    public bool TryRegisterSignature(string name, params Type[] args)
    {
        var type = Expression.GetDelegateType(args);
        return _signatures.TryAdd(NameResolvingPolicy.Resolve(name), (type, args));
    }
    
    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        args ??= Array.Empty<object?>();

        var name = NameResolvingPolicy.Resolve(binder.Name);
        
        if (!_signatures.TryGetValue(name, out var signature))
        {
            result = null;
            return false;
        }

        if (signature.TypeArgs.Length - 1 != args.Length)
        {
            result = null;
            return false;
        }

        var fp = System.Runtime.InteropServices.NativeLibrary.GetExport(_module, name);

        if (!_delegates.TryGetValue(name, out var func))
        {
            func = Marshal.GetDelegateForFunctionPointer(fp, signature.Signature);
            _delegates.Add(name, func);
        }

        result = func.DynamicInvoke(args);
        return true;
    }

    public void Dispose()
    {
        System.Runtime.InteropServices.NativeLibrary.Free(_module);
    }
}
