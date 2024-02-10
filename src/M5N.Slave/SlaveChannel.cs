using System.Net;
using M5N.DataTransferObjects;
using M5N.Primitives;

namespace M5N.Slave;

public class SlaveChannel(EndPoint endpoint, Slave slave) : Channel(endpoint)
{
    private byte _id = 0xFF;

    private bool Frame()
    {
        var timeout = Timeout.InfiniteTimeSpan;

        ReceiveCode(ref timeout, out var ctrl, out var code);

        var tag = (TagCode)code;
        if (ctrl is ControlCode.Syn)
        {
            ErrorCode err;
            switch (tag)
            {
                case TagCode.Colour:
                    var colour = slave.InqueryColour();
                    err = Respond(new ColourDTO(colour));
                    if (err is ErrorCode.Success)
                        slave.Colour = colour;
                    break;
                case TagCode.Coordinate:
                    var (x, y) = slave.InqueryStone();
                    err        = Respond(new CoordinateDTO(x, y, slave.Colour));
                    if (err is ErrorCode.Success)
                        slave.SetStone(x, y, slave.Colour);
                    break;
                case TagCode.Identifier:
                    err = Respond(new IdentifierDTO(_id));
                    break;
                case TagCode.Choice:
                    err = Respond(new ChoiceDTO(slave.MakeDecision()));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (err is not ErrorCode.Success)
                throw new InvalidOperationException(Enum.GetName(err));
        }
        else if (ctrl is ControlCode.SynAck)
        {
            switch (tag)
            {
                case TagCode.Colour:
                {
                    using var ctx = Listen<ColourDTO>(timeout);
                    slave.Colour = ctx.Result!.Value.Colour;
                    ctx.Status   = ErrorCode.Success;
                    break;
                }
                case TagCode.Coordinate:
                {
                    using var ctx = Listen<CoordinateDTO>(timeout);
                    var       dto = ctx.Result!.Value;
                    slave.SetStone(dto.X, dto.Y, dto.Colour);
                    ctx.Status = ErrorCode.Success;
                    break;
                }
                case TagCode.Identifier:
                {
                    using var ctx = Listen<IdentifierDTO>(timeout);

                    if (_id == 0xFF)
                        _id = ctx.Result!.Value.Identifier;
                    else if (ctx.Result!.Value.Identifier == _id)
                    {
                        slave.DeclareVictory();
                        return false;
                    }
                    else
                    {
                        slave.DeclareDefeat();
                        return false;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return true;
    }

    public override void Run()
    {
        while (Frame())
        {
        }
    }
}