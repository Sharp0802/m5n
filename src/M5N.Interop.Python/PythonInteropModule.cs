using System.Dynamic;
using M5N.Interop.Python.Converters;

namespace M5N.Interop.Python;

public class PythonInteropModule : InteropModule
{
    public override INameResolvingPolicy NameResolvingPolicy { get; set; } = new SnakeCaseNameResolvingPolicy();

    private readonly IntPtr                     _module;
    private readonly Dictionary<string, IntPtr> _func = new();

    internal PythonInteropModule(string name)
    {
        _module = PyMarshal.ImportModule(name);
        if (_module == IntPtr.Zero)
            throw new PythonException();
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        var name = NameResolvingPolicy.Resolve(binder.Name);
        
        if (!_func.TryGetValue(name, out var func))
        {
            func = PyMarshal.GetAttrString(_module, name);
            _func.Add(name, func);
        }

        if (func == IntPtr.Zero)
        {
            result = null;
            return false;
        }

        var tuple = ObjectConverter.Default.Serialize(args);
        var obj = PyMarshal.CallObject(func, tuple);

        if (!ObjectConverter.Default.CanDeserialize(obj))
        {
            result = null;
            return false;
        }
        
        result = ObjectConverter.Default.Deserialize(obj);
        return true;
    }
}
