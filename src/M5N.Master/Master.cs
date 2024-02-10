using System.Runtime.CompilerServices;
using M5N.DataTransferObjects;
using M5N.Primitives;

namespace M5N.Master;

public class Master
{
    public Master(MasterChannel user0, MasterChannel user1)
    {
        User0 = user0;
        User1 = user1;
    }


    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    public MasterChannel User0 { get; }
    public MasterChannel User1 { get; }
    

    private Colour[,] Map { get; } = new Colour[15, 15];

    private byte Winner { get; set; } = 0xFF;

    private MasterChannel this[byte id] => id == 0 ? User0 : User1;


    private byte OtherPlayer(byte id) => (byte)((id + 1) % 2);

    private void DeclareVictory(byte id)
    {
        User0.Respond(new IdentifierDTO(id));
        User1.Respond(new IdentifierDTO(id));
        Winner = id;
    }

    private ChannelRespondContext<T> AcceptDTOFrom<T>(byte id) where T : unmanaged, IChannelObject<T>
    {
        var ctx = this[id].Request<T>(Timeout);
        if (ctx.Result is null) 
            DeclareVictory(OtherPlayer(id));

        return ctx;
    }

    /// <returns>True if game should be continued; otherwise, False.</returns>
    private bool AcceptCoordinateFrom(byte id, out CoordinateDTO coord)
    {
        Unsafe.SkipInit(out coord);
        
        var other = OtherPlayer(id);

        using var ctx = AcceptDTOFrom<CoordinateDTO>(id);
        if (ctx.Result is null)
            return false;

        var dto = ctx.Result.Value;
        if (dto.X > 14 || dto.Y > 14 || Map[dto.X, dto.Y] != Colour.None)
        {
            ctx.Status = ErrorCode.Invalid;
            DeclareVictory(other);
            return false;
        }

        ctx.Status        = ErrorCode.Success;
        coord             = dto;
        Map[dto.X, dto.Y] = dto.Colour;
        this[other].Respond(dto);
        
        return true;
    }

    /// <returns><inheritdoc cref="AcceptCoordinateFrom"/></returns>
    private bool AcceptChoiceFrom(byte id, out TagCode choice)
    {
        Unsafe.SkipInit(out choice);
        
        var other = OtherPlayer(id);
        
        using var ctx = AcceptDTOFrom<ChoiceDTO>(id);
        if (ctx.Result is null)
            return false;

        if (ctx.Result.Value.Tag is not (TagCode.Colour or TagCode.Coordinate))
        {
            ctx.Status = ErrorCode.Invalid;
            DeclareVictory(other);
            return false;
        }

        ctx.Status = ErrorCode.Success;
        choice     = ctx.Result.Value.Tag;
        return true;
    }

    /// <returns><inheritdoc cref="AcceptCoordinateFrom"/></returns>
    private bool AcceptColourFrom(byte id, out Colour colour)
    {
        Unsafe.SkipInit(out colour);
        
        var other = OtherPlayer(id);

        using var ctx = AcceptDTOFrom<ColourDTO>(id);
        if (ctx.Result is null)
            return false;

        if (ctx.Result.Value.Colour is not (Colour.Black or Colour.White))
        {
            ctx.Status = ErrorCode.Invalid;
            DeclareVictory(other);
            return false;
        }

        ctx.Status = ErrorCode.Success;
        colour     = ctx.Result.Value.Colour;
        return true;
    }

    private void SwapColour()
    {
        (User0.Colour, User1.Colour) = (User1.Colour, User0.Colour);
        User0.Respond(new ColourDTO(User0.Colour));
        User1.Respond(new ColourDTO(User1.Colour));
    }

    private int LengthFrom(CoordinateDTO coord, int offsetX, int offsetY)
    {
        var size = Map.GetLength(0);

        var x = (int)coord.X;
        var y = (int)coord.Y;

        var cnt = 0;
        for (; 
             0 <= x && x < size && 
             0 <= y && y < size &&
             Map[x, y] == coord.Colour; 
             x += offsetX,
             y += offsetY)
            cnt++;
        return cnt;
    }

    private int BiLengthFrom(CoordinateDTO coord, int offsetX, int offsetY)
    {
        return LengthFrom(coord, offsetX, offsetY) + LengthFrom(coord, -offsetX, -offsetY) - 1;
    }

    private int RankOf(CoordinateDTO coord)
    {
        return new[]
        {
            BiLengthFrom(coord, 0, 1),
            BiLengthFrom(coord, 1, 1),
            BiLengthFrom(coord, 1, 0),
            BiLengthFrom(coord, 1, -1)
        }.Max();
    }

    public int Run()
    {
        User0.Respond(new IdentifierDTO(0));
        User1.Respond(new IdentifierDTO(1));
        
        for (var i = 0; i < 3; ++i)
        {
            SwapColour();
            if (!AcceptCoordinateFrom(0, out _))
                return Winner;
        }

        if (!AcceptChoiceFrom(1, out var choice))
            return Winner;

        byte current;
        if (choice == TagCode.Coordinate)
        {
            SwapColour();
            for (var i = 0; i < 2; ++i)
            {
                SwapColour();
                if (!AcceptCoordinateFrom(1, out _))
                    return Winner;
            }
            
            if (!AcceptColourFrom(0, out var colour))
                return Winner;
            if (colour != User0.Colour)
                SwapColour();
            
            current = (byte)(colour is Colour.White ? 0 : 1);
        }
        else
        {
            if (!AcceptColourFrom(1, out var colour))
                return Winner;
            if (colour != User1.Colour)
                SwapColour();
            
            current = (byte)(colour is Colour.White ? 1 : 0);
        }

        while (true)
        {
            if (!AcceptCoordinateFrom(current, out var coord))
                return Winner;

            if (RankOf(coord) >= 5)
            {
                DeclareVictory(current);
                return Winner;
            }

            current = OtherPlayer(current);
        }
    }
}