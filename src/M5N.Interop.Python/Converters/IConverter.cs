namespace M5N.Interop.Python.Converters;

public interface IConverter
{
    public bool CanDeserialize(IntPtr obj);

    public bool CanSerialize(object? obj);

    public object? Deserialize(IntPtr obj);

    public IntPtr Serialize(object? obj);
}

public interface IConverter<T> : IConverter
{
    public new T? Deserialize(IntPtr obj);

    public IntPtr Serialize(T? obj);
}