namespace M5N.Interop.Python.Converters;

public class StringConverter : ConverterBase<string>
{
    public override bool CanDeserialize(IntPtr obj)
    {
        return PyMarshal.IsSubclassOf(obj, PyMarshal.TPFlags.UnicodeSubclass);
    }

    public override string Deserialize(IntPtr obj)
    {
        return PyMarshal.AsString(obj);
    }

    public override IntPtr Serialize(string? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return PyMarshal.FromString(obj);
    }
}