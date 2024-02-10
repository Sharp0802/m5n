using System.Diagnostics.CodeAnalysis;

namespace M5N.Slave.Interop.Converters;

public interface IValueConverter<T>
{
    public static abstract IntPtr Serialize(T        cs);
    public static abstract bool   Deserialize(IntPtr py, [NotNullWhen(true)] out T? result);
}