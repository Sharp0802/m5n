using M5N.Primitives;
using M5N.Slave.Interop;
using M5N.Slave.Interop.Dynamics;

namespace M5N.Slave;

public class Slave(Module module)
{
    private readonly Module.PyFunctionDelegate _setColour      = GetMethod(module, "set_colour");
    private readonly Module.PyFunctionDelegate _setStone       = GetMethod(module, "set_stone");
    private readonly Module.PyFunctionDelegate _chooseColour   = GetMethod(module, "choose_colour");
    private readonly Module.PyFunctionDelegate _placeStone     = GetMethod(module, "place_stone");
    private readonly Module.PyFunctionDelegate _makeDecision   = GetMethod(module, "make_decision");
    private readonly Module.PyFunctionDelegate _declareVictory = GetMethod(module, "victory");
    private readonly Module.PyFunctionDelegate _declareDefeat  = GetMethod(module, "defeat");

    private static Module.PyFunctionDelegate GetMethod(Module module, string name)
    {
        return module.GetMethod(name) ?? throw new MissingMethodException(null, name);
    }

    private Colour _colour;

    public Colour Colour
    {
        get => _colour;
        set
        {
            _colour = value;
            _setColour((long)value);
        }
    }

    public Colour InqueryColour()
    {
        return (Colour)((PythonInteger)_chooseColour()).Value;
    }

    public void SetStone(byte x, byte y, Colour colour)
    {
        _setStone(x, y, (long)colour);
    }

    public (byte X, byte Y) InqueryStone()
    {
        var tuple = (PythonTuple)_placeStone();
        if (tuple.Value.Length != 2)
            throw new InvalidProgramException("Signature of reserved function is modified. See the manual.");
        if (tuple.Value.Any(obj => obj is null))
            throw new NullReferenceException("Element of coordinate cannot be null.");

        var coord = (
            X: ((PythonInteger)tuple.Value[0]!).Value,
            Y: ((PythonInteger)tuple.Value[1]!).Value
        );

        if (coord.X < 0 || 15 <= coord.X ||
            coord.Y < 0 || 15 <= coord.Y)
            throw new InvalidOperationException();

        return ((byte)coord.X, (byte)coord.Y);
    }

    public TagCode MakeDecision()
    {
        return (TagCode)((PythonInteger)_makeDecision()).Value;
    }

    public void DeclareVictory()
    {
        _declareVictory();
    }

    public void DeclareDefeat()
    {
        _declareDefeat();
    }
}