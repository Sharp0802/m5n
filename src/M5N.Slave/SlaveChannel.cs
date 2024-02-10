using System.Net;

namespace M5N.Slave;

public class SlaveChannel : Channel
{
    public SlaveChannel(EndPoint endpoint, Slave slave) : base(endpoint)
    {
    }

    public override void Run()
    {
        throw new NotImplementedException();
    }
}