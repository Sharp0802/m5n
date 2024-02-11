using System.Numerics;

namespace M5N.Interop.Python.Converters;

public class IntegerConverter<T> : ConverterBase<T> where T : IBinaryInteger<T>
{
    static IntegerConverter()
    {
        if (T.AllBitsSet.GetByteCount() > sizeof(long))
            throw new InsufficientMemoryException("Type size should be less or equal than Int64.");
    }
    
    
    public override bool CanDeserialize(IntPtr obj)
    {
        return PyMarshal.IsSubclassOf(obj, PyMarshal.TPFlags.LongSubclass);
    }

    public override T Deserialize(IntPtr obj)
    {
        return T.CreateChecked(PyMarshal.AsInt64(obj));
    }

    public override IntPtr Serialize(T? obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));
        return PyMarshal.FromInt64(long.CreateChecked(obj));
    }
}