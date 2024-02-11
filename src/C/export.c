#include <Python.h>

__attribute__((visibility("default")))
void* GetAddressOfNoneObject()
{
    return Py_None;
}
