namespace M5N.Interop.Python;

public struct PyNativeObject
{
#if !Py_GIL_DISABLED
    public nint   RefCount;
    public IntPtr Type;
#else
    public UIntPtr Tid;
    public ushort  Padding;
    public byte    Mutex;
    public byte    GCBits;
    public uint    RefLocal;
    public nint    RefShared;
    public IntPtr  Type;
#endif
}