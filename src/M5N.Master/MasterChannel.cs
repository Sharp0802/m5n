using System.Net.Sockets;

namespace M5N.Master;

public class MasterChannel(Socket server) : Channel(server)
{
    public override void Run()
    {
    }
}