using M5N.Slave.Interop.Converters;

namespace M5N.Slave.Interop.Dynamics;

public class PythonInteger : PythonObject, IPythonObject<long, PythonInteger>
{
    private PythonInteger(long value)
    {
        Value  = value;
        Handle = IntegerConverter.Serialize(value);
    }

    public override Type Type => typeof(long);

    public override IntPtr Handle { get; }
    public          long   Value  { get; }

    public static implicit operator PythonInteger(IntPtr raw)
    {
        if (!IntegerConverter.Deserialize(raw, out var @int))
            throw new InvalidCastException();
        return @int;
    }

    public static implicit operator PythonInteger(long raw)
    {
        return new PythonInteger(raw);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}