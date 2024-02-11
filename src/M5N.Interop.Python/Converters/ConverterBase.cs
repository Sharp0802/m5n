namespace M5N.Interop.Python.Converters;

public abstract class ConverterBase<T> : IConverter<T>
{
    public abstract bool CanDeserialize(IntPtr obj);

    public virtual bool CanSerialize(object? obj) => obj is T;

    IntPtr IConverter. Serialize(object?  obj) => Serialize((T?)obj);
    object? IConverter.Deserialize(IntPtr obj) => Deserialize(obj);

    public abstract T?     Deserialize(IntPtr obj);
    public abstract IntPtr Serialize(T?       obj);
}