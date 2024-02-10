using M5N.Primitives;
using M5N.Slave.Interop;
using M5N.Slave.Interop.Dynamics;

namespace M5N.Slave;

public class Slave(Module module)
{
    private readonly Module.PyFunctionDelegate _setColour    = GetMethod(module, "set_colour");
    private readonly Module.PyFunctionDelegate _setStone     = GetMethod(module, "set_stone");
    private readonly Module.PyFunctionDelegate _chooseColour = GetMethod(module, "choose_colour");
    private readonly Module.PyFunctionDelegate _placeStone   = GetMethod(module, "place_stone");

    private static Module.PyFunctionDelegate GetMethod(Module module, string name)
    {
        return module.GetMethod(name) ?? throw new MissingMethodException(null, name);
    }

    public void SetColour(Colour colour)
    {
        _setColour((long)colour);
    }

    public Colour InqueryColour()
    {
        return (Colour)((PythonInteger)_chooseColour()).Value;
    }

    public void SetStone(byte x, byte y, Colour colour)
    {
        _setStone(x, y, (long)colour);
    }

    public (long, long) InqueryStone()
    {
        var tuple = (PythonTuple)_placeStone();
        if (tuple.Value.Length != 2)
            throw new InvalidProgramException("Signature of reserved function is modified. See the manual.");
        if (tuple.Value.Any(obj => obj is null))
            throw new NullReferenceException("Element of coordinate cannot be null.");

        return (
            ((PythonInteger)tuple.Value[0]!).Value,
            ((PythonInteger)tuple.Value[1]!).Value
        );
    }
}