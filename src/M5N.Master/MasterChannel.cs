using System.Net.Sockets;

namespace M5N.Master;

public class MasterChannel : Channel
{
    public MasterChannel(Socket server) : base(server)
    {
    }

    public override void Run()
    {
        throw new NotImplementedException();
    }
}