using M5N.Slave.Interop.Converters;

namespace M5N.Slave.Interop.Dynamics;

public class PythonTuple : PythonObject, IPythonObject<PythonObject?[], PythonTuple>
{
    private PythonTuple(PythonObject?[] value)
    {
        Value  = value;
        Handle = TupleConverter.Serialize(value);
    }

    public override Type            Type   { get; } = typeof(PythonObject[]);
    public override IntPtr          Handle { get; }
    public          PythonObject?[] Value  { get; }

    public static implicit operator PythonTuple(IntPtr raw)
    {
        if (!TupleConverter.Deserialize(raw, out var result))
            throw new InvalidCastException();
        return result;
    }

    public static implicit operator PythonTuple(PythonObject?[] raw)
    {
        return new PythonTuple(raw);
    }
}
