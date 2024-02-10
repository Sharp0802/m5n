using System.Net.Sockets;
using M5N.Primitives;

namespace M5N.Master;

public class MasterChannel(Socket server, Colour colour) : Channel(server)
{
    public Colour Colour { get; set; } = colour;
    
    public override void Run()
    {
    }
}