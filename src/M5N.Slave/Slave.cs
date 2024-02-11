using M5N.Interop.NativeLibrary;
using M5N.Primitives;

namespace M5N.Slave;

public class Slave(dynamic module)
{
    private Colour _colour;

    public Colour Colour
    {
        get => _colour;
        set
        {
            _colour = value;
            module.SetColour((byte)value);
        }
    }

    public Colour InqueryColour()
    {
        return (Colour)module.ChooseColour();
    }

    public void SetStone(byte x, byte y, Colour colour)
    {
        module.SetStone(x, y, (byte)colour);
    }

    public (byte X, byte Y) InqueryStone()
    {
        var tuple = ((IEnumerable<object?>)module.PlaceStone()).ToArray();
        if (tuple.Length != 2)
            throw new InvalidProgramException("Signature of reserved function is modified. See the manual.");
        if (tuple.Any(obj => obj is null))
            throw new NullReferenceException("Element of coordinate cannot be null.");

        var coord = (
            X: (long)Convert.ChangeType(tuple[0]!, TypeCode.Int64),
            Y: (long)Convert.ChangeType(tuple[1]!, TypeCode.Int64)
        );

        if (coord.X < 0 || 15 <= coord.X ||
            coord.Y < 0 || 15 <= coord.Y)
            throw new InvalidOperationException();

        return ((byte)coord.X, (byte)coord.Y);
    }

    public TagCode MakeDecision()
    {
        return (TagCode)module.MakeDecision();
    }

    public void DeclareVictory()
    {
        module.Victory();
    }

    public void DeclareDefeat()
    {
        module.Defeat();
    }
}