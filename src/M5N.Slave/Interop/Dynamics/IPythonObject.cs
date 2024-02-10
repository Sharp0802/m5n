namespace M5N.Slave.Interop.Dynamics;

public interface IPythonObject
{
    public Type Type { get; }

    public IntPtr Handle { get; }
}

public interface IPythonObject<out TValue> : IPythonObject
{
    public TValue Value { get; }
}

public interface IPythonObject<TValue, TSelf>
    : IPythonObject<TValue>
    where TSelf : PythonObject, IPythonObject<TValue, TSelf>
{
    public static abstract implicit operator TSelf(IntPtr raw);

    public static abstract implicit operator TSelf(TValue raw);
    public static virtual implicit operator  TValue(TSelf self) => self.Value;
}