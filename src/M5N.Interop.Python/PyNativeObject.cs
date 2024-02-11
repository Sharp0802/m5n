namespace M5N.Interop.Python;

public struct PyNativeObject
{
    /*
    public UIntPtr Tid;
    public ushort  Padding;
    public byte    Mutex;
    public byte    GCBits;
    public uint    RefLocal;
    public nint    RefShared;
    public IntPtr  Type;
    */
    public nint    RefCount;
    public IntPtr  Type;
}