using System.Net;
using System.Text;
using M5N.Slave;
using M5N.Slave.Interop;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding  = Encoding.UTF8;

if (args.Length is not (2 or 3))
{
    Console.Error.WriteLine("Invalid syntax.");
    return 1;
}

EndPoint ep;
if (IPEndPoint.TryParse(args[0], out var ipEp))
{
    ep = ipEp;
}
else
{
    var trunc = args[0].Split(':');
    if (trunc.Length != 2)
    {
        Console.Error.WriteLine("Invalid DNS format.");
        return 1;
    }

    if (ushort.TryParse(trunc[1], out var port))
    {
        Console.Error.WriteLine("Invalid port number.");
        return 1;
    }

    ep = new DnsEndPoint(trunc[0], port);
}

var root   = args[1];
var module = args.Length == 3 ? args[2] : "main";

using (var gil = new Python(root))
{
    var slave = new Slave(gil.Import(module));
    using var channel = new SlaveChannel(ep, slave);
    Console.WriteLine("Connection established.");
    channel.Run();
    Console.WriteLine("Connection closed.");
}

return 0;
