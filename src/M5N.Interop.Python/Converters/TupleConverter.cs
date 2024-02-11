using System.Collections;

namespace M5N.Interop.Python.Converters;

public class TupleConverter : ConverterBase<IEnumerable<object?>>
{
    public override bool CanDeserialize(IntPtr obj)
    {
        if (!PyMarshal.IsSubclassOf(obj, PyMarshal.TPFlags.TupleSubclass))
            return false;

        var size = PyMarshal.TupleSizeOf(obj);
        for (var i = 0; i < size; ++i)
        {
            var item = PyMarshal.GetItemFromTuple(obj, i);
            if (!ObjectConverter.Default.CanDeserialize(item))
                return false;
        }

        return true;
    }

    public override bool CanSerialize(object? obj)
    {
        if (obj is null)
            return false;
        if (!base.CanSerialize(obj))
            return false;

        return ((IEnumerable)obj).OfType<object>().All(item => ObjectConverter.Default.CanSerialize(item));
    }

    public override IEnumerable<object?> Deserialize(IntPtr obj)
    {
        var size = PyMarshal.TupleSizeOf(obj);
        for (var i = 0; i < size; ++i)
        {
            var item = PyMarshal.GetItemFromTuple(obj, i);
            if (!ObjectConverter.Default.CanDeserialize(item))
                throw new InvalidCastException();

            yield return ObjectConverter.Default.Deserialize(item);
        }
    }

    public override IntPtr Serialize(IEnumerable<object?>? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        
        var arr = obj as object?[] ?? obj.ToArray();

        var tuple = PyMarshal.NewTuple(arr.Length);
        for (var i = 0; i < arr.Length; ++i)
        {
            var item = arr[i];
            if (item is not null && !ObjectConverter.Default.CanSerialize(item))
                throw new InvalidCastException();

            PyMarshal.SetItemOfTuple(tuple, i, ObjectConverter.Default.Serialize(item));
        }

        return tuple;
    }
}