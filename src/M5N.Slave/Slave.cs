using M5N.Logging;
using M5N.Primitives;
using M5N.Slave.Shared;
using Microsoft.Extensions.Logging;
using LoggerFactory = M5N.Logging.LoggerFactory;

namespace M5N.Slave;

public class Slave(dynamic module) : ISlave, ITraceable<Slave>
{
    public ILogger<Slave> Logger { get; } = LoggerFactory.Acquire<Slave>();

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

    public Colour InquiryColour()
    {
        Log.CallerMember(this);
        
        return (Colour)module.ChooseColour();
    }

    public void SetStone(byte x, byte y, Colour colour)
    {
        Log.CallerMember(this);

        module.SetStone(x, y, (byte)colour);
    }

    public (byte X, byte Y) InquiryStone()
    {
        Log.CallerMember(this);

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
        Log.CallerMember(this);

        return (TagCode)module.MakeDecision();
    }

    public void DeclareVictory()
    {
        Log.CallerMember(this);

        module.Victory();
    }

    public void DeclareDefeat()
    {
        Log.CallerMember(this);

        module.Defeat();
    }
}