#include <Python.h>

__attribute__((visibility("default")))
void* GetAddressOfNoneObject()
{
    return Py_None;
}

__attribute__((visibility("default")))
int IsType_PythonString(void* py)
{
    return PyUnicode_Check(py);
}

__attribute__((visibility("default")))
int IsType_PythonInteger(void* py)
{
    return PyLong_Check(py);
}
