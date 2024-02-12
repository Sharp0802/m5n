namespace M5N.Interop.Python.Converters;

public class ObjectConverter : ConverterBase<object>
{
    private static IEnumerable<IConverter> Converters =>
    [
        new StringConverter(),
        
        new IntegerConverter<byte>(),
        new IntegerConverter<ushort>(),
        new IntegerConverter<uint>(),
        new IntegerConverter<nuint>(),
        new IntegerConverter<ulong>(),
        
        new IntegerConverter<sbyte>(),
        new IntegerConverter<short>(),
        new IntegerConverter<int>(),
        new IntegerConverter<nint>(),
        new IntegerConverter<long>(),
        
        new TupleConverter()
    ];

    public static ObjectConverter Default { get; } = new();
    
    public static IntPtr NoneHandle { get; }

    static ObjectConverter()
    {
        NoneHandle = PyMarshal.GetAddressOfNoneObject();
    }

    public override bool CanDeserialize(IntPtr obj)
    {
        return obj == IntPtr.Zero ||
               obj == NoneHandle ||
               Converters.Any(cvt => cvt.CanDeserialize(obj));
    }

    public override bool CanSerialize(object? obj)
    {
        return obj is null || 
               Converters.Any(cvt => cvt.CanSerialize(obj));
    }

    public override object? Deserialize(IntPtr obj)
    {
        return obj == NoneHandle || obj == IntPtr.Zero
            ? null 
            : Converters.First(cvt => cvt.CanDeserialize(obj)).Deserialize(obj);
    }

    public override IntPtr Serialize(object? obj)
    {
        return obj is null 
            ? NoneHandle 
            : Converters.First(cvt => cvt.CanSerialize(obj)).Serialize(obj);
    }
}