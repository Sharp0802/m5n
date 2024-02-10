using M5N.Slave.Interop.Converters;

namespace M5N.Slave.Interop.Dynamics;

public class PythonString : PythonObject, IPythonObject<string, PythonString>
{
    private PythonString(string value)
    {
        Value  = value;
        Handle = StringConverter.Serialize(value);
    }

    public override Type Type => typeof(string);

    public override IntPtr Handle { get; }
    public          string Value  { get; }

    public static implicit operator PythonString(IntPtr raw)
    {
        if (!StringConverter.Deserialize(raw, out var str))
            throw new InvalidCastException();
        return str;
    }

    public static implicit operator PythonString(string raw)
    {
        return new PythonString(raw);
    }

    public override string ToString()
    {
        return Value;
    }
}