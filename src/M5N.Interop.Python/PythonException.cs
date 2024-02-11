using System.Runtime.InteropServices;

namespace M5N.Interop.Python;

public class PythonException()
    : ExternalException(PyMarshal.AsString(PyMarshal.ToString(PyMarshal.GetRaisedException())));
    