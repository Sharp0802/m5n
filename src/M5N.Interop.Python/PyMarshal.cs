using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace M5N.Interop.Python;

internal static class PyMarshal
{
    private const string Python    = "PY_INTEROP_MODULE";
    private const string FnExports = "pyexport";


    static PyMarshal()
    {
        // On windows, there is python311.dll instead of libpython3.11.so
        NativeLibrary.SetDllImportResolver(typeof(PyMarshal).Assembly, (name, _, _) =>
        {
            var handle = IntPtr.Zero;
            if (name.Equals(Python, StringComparison.Ordinal))
                handle = LoadPython();
            return handle;
        });
    }
    
    private static IntPtr LoadPython()
    {
        return OperatingSystem.IsWindows()
            ? NativeLibrary.Load("python311")
            : NativeLibrary.Load("python3.11");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Initialize()
    {
        Py_Initialize();
        return;

        [DllImport(Python)]
        static extern void Py_Initialize();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Uninitialize()
    {
        return Py_FinalizeEx();

        [DllImport(Python)]
        static extern int Py_FinalizeEx();
    }

    internal static IntPtr ImportModule(string name)
    {
        unsafe
        {
            var len = Encoding.UTF8.GetByteCount(name);
            var dst = stackalloc byte[len + 1];
            fixed (char* src = name)
                Encoding.UTF8.GetBytes(src, name.Length, dst, len);
            dst[len] = 0;

            return PyImport_ImportModule(dst);

            [DllImport(Python)]
            static extern IntPtr PyImport_ImportModule(byte* name);
        }
    }

    internal static IntPtr GetAttrString(IntPtr obj, string name)
    {
        unsafe
        {
            var len = Encoding.UTF8.GetByteCount(name);
            var dst = stackalloc byte[len + 1];
            fixed (char* src = name)
                Encoding.UTF8.GetBytes(src, name.Length, dst, len);
            dst[len] = 0;

            return PyObject_GetAttrString(obj, dst);

            [DllImport(Python)]
            static extern IntPtr PyObject_GetAttrString(IntPtr o, byte* name);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr CallObject(IntPtr callable, IntPtr args)
    {
        return PyObject_CallObject(callable, args);

        [DllImport(Python)]
        static extern IntPtr PyObject_CallObject(IntPtr callable, IntPtr args);
    }

    internal static int RunSimpleString(string str)
    {
        unsafe
        {
            var len = Encoding.UTF8.GetByteCount(str);
            var dst = stackalloc byte[len + 1];
            fixed (char* src = str)
                Encoding.UTF8.GetBytes(src, str.Length, dst, len);
            dst[len] = 0;

            return PyRun_SimpleString(dst);

            [DllImport(Python)]
            static extern int PyRun_SimpleString(byte* name);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr GetRaisedException()
    {
        return PyErr_GetRaisedException();

        [DllImport(Python)]
        static extern IntPtr PyErr_GetRaisedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr ToString(IntPtr obj)
    {
        return PyObject_Str(obj);

        [DllImport(Python)]
        static extern IntPtr PyObject_Str(IntPtr o);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr FromInt64(long v)
    {
        return PyLong_FromLongLong(v);

        [DllImport(Python)]
        static extern IntPtr PyLong_FromLongLong(long v);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long AsInt64(IntPtr v)
    {
        return PyLong_AsLongLong(v);

        [DllImport(Python)]
        static extern long PyLong_AsLongLong(IntPtr v);
    }

    internal static IntPtr FromString(string str)
    {
        unsafe
        {
            var len = Encoding.UTF8.GetByteCount(str);
            var dst = stackalloc byte[len];
            fixed (char* src = str)
                Encoding.UTF8.GetBytes(src, str.Length, dst, len);

            return PyUnicode_FromStringAndSize(dst, len);

            [DllImport(Python)]
            static extern IntPtr PyUnicode_FromStringAndSize(byte* v, nint len);
        }
    }

    internal static string AsString(IntPtr str)
    {
        unsafe
        {
            return Marshal.PtrToStringUTF8((IntPtr)PyUnicode_AsUTF8(str))
                ?? throw new InvalidOperationException("Couldn't convert lpsz to managed string");

            [DllImport(Python)]
            static extern byte* PyUnicode_AsUTF8(IntPtr v);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr NewTuple(int n)
    {
        return PyTuple_New(n);

        [DllImport(Python)]
        static extern IntPtr PyTuple_New(nint n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr GetItemFromTuple(IntPtr tuple, nint pos)
    {
        return PyTuple_GetItem(tuple, pos);

        [DllImport(Python)]
        static extern IntPtr PyTuple_GetItem(IntPtr tuple, nint pos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IntPtr SetItemOfTuple(IntPtr tuple, nint pos, IntPtr obj)
    {
        return PyTuple_SetItem(tuple, pos, obj);

        [DllImport(Python)]
        static extern IntPtr PyTuple_SetItem(IntPtr tuple, nint pos, IntPtr obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static nint TupleSizeOf(IntPtr tuple)
    {
        return PyTuple_Size(tuple);

        [DllImport(Python)]
        static extern nint PyTuple_Size(IntPtr tuple);
    }

    private static TPFlags GetTypeFlags(IntPtr type)
    {
        const string entry = "PyType_GetFlags";
        
        return (TPFlags)(OperatingSystem.IsWindows()
            ? PyType_GetFlags_Windows(type)
            : PyType_GetFlags_Unix(type));

        [SupportedOSPlatform("windows")]
        [DllImport(Python, EntryPoint = entry)]
        static extern int PyType_GetFlags_Windows(IntPtr type);

        [UnsupportedOSPlatform("windows")]
        [DllImport(Python, EntryPoint = entry)]
        static extern nint PyType_GetFlags_Unix(IntPtr type);
    }

    [Flags]
    internal enum TPFlags : long
    {
        SubclassField = 0xFF_00_00_00L,
        
        LongSubclass    = 1L << 24,
        ListSubclass    = 1L << 25,
        TupleSubclass   = 1L << 26,
        BytesSubclass   = 1L << 27,
        UnicodeSubclass = 1L << 28,
        DictSubclass    = 1L << 29,
        BaseExcSubclass = 1L << 30,
        TypeSubclass    = 1L << 31
    }

    internal static bool FastSubclass(IntPtr type, TPFlags feature)
    {
        return (GetTypeFlags(type) & feature) == feature;
    }

    internal static IntPtr GetType(IntPtr obj)
    {
        unsafe
        {
            return ((PyNativeObject*)obj)->Type;
        }
    }

    internal static bool IsSubclassOf(IntPtr obj, TPFlags type)
    {
        return FastSubclass(GetType(obj), type);
    }

    [DllImport(FnExports)]
    internal static extern IntPtr GetAddressOfNoneObject();
}