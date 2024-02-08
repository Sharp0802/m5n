using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace M5N.Slave;

public partial class Python
{
    private const string Library = "python312";

    [LibraryImport(Library)]
    private static partial void Py_Initialize();

    [LibraryImport(Library)]
    private static unsafe partial void* PyImport_ImportModule(byte* name);

    [LibraryImport(Library)]
    private static unsafe partial void* PyObject_GetAttrString(void* o, byte* name);

    [LibraryImport(Library)]
    private static partial int Py_FinalizeEx();

    [DllImport(Library)]
    private static extern unsafe void* Py_BuildValue(byte* type, __arglist);

    [LibraryImport(Library)]
    private static unsafe partial int PyRun_SimpleString(byte* name);

    [DllImport(Library)]
    private static extern unsafe void* PyTuple_Pack(nint n, __arglist);

    [LibraryImport(Library)]
    private static unsafe partial void* PyObject_CallObject(void* callable, void* args);
    
    private static byte[] GetLPZSTR(string data, Encoding? enc = null)
    {
        enc ??= Encoding.UTF8;
        
        var bytes = new byte[enc.GetByteCount(data) + 1];
        enc.GetBytes(data, bytes);
        bytes[^1] = 0;

        return bytes;
    }

    private unsafe void* ToPythonObject(object value)
    {
        var dict = new Dictionary<Type, string>
        {
            [typeof(string)] = "s",
            [typeof(byte[])] = "y#",

            [typeof(sbyte)]  = "b",
            [typeof(short)]  = "h",
            [typeof(int)]    = "i",
            [typeof(nint)]   = "l",
            [typeof(long)]   = "L",
            [typeof(byte)]   = "B",
            [typeof(ushort)] = "H",
            [typeof(uint)]   = "I",
            [typeof(nuint)]  = "k",
            [typeof(ulong)]  = "K",

            [typeof(double)] = "d",
            [typeof(float)]  = "f"
        };

        void* py;
        fixed (byte* t = GetLPZSTR(dict[value.GetType()]))
        {
            switch (value)
            {
                case string str:
                {
                    fixed (byte* p = GetLPZSTR(str))
                        py = Py_BuildValue(t, __arglist(p));
                    break;
                }
                case byte[] arr:
                {
                    fixed (byte* p = arr)
                        py = Py_BuildValue(t, __arglist(p, (nuint)arr.Length));
                    break;
                }
                default:
                    py = Py_BuildValue(t, __arglist(value));
                    break;
            }
        }

        return py;
    }
    
    public static int Call(string directory, string module, string func)
    {
        Py_Initialize();

        int r;
        unsafe
        {
            fixed (byte* src = GetLPZSTR($"import sys;sys.path.append('{directory}')"))
                r = PyRun_SimpleString(src);
            if (r != 0)
                goto EXIT;
            
            /*
            fixed (byte* type = "s"u8)
            fixed (byte* mode = "r"u8)
            fixed (byte* p = GetLPZSTR(module))
            {
                var obj = Py_BuildValue(type, p);
                var fp = _Py_fopen_obj(obj, mode);
                if (fp is null)
                    throw new FileNotFoundException();
                
                r = PyRun_SimpleFile(fp, p);
            }
            */

            void* main;
            fixed (byte* p = GetLPZSTR(module))
                main = PyImport_ImportModule(p);

            void* fp;
            fixed (byte* p = GetLPZSTR(func))
                fp = PyObject_GetAttrString(main, p);

            var args = PyTuple_Pack(0, __arglist());

            PyObject_CallObject(fp, args);
        }

        EXIT:
        Py_FinalizeEx();
        
        return r;
    }
}