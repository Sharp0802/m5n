
using System.Net;
using System.Net.Sockets;
using M5N.Master;

if (args.Length != 1)
{
    Console.Error.WriteLine("Invalid syntax.");
    return 1;
}

if (!ushort.TryParse(args[0], out var port))
{
    Console.Error.WriteLine("Invalid port number.");
    return 1;
}

using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
{
    socket.Bind(new IPEndPoint(IPAddress.Any, port));
    socket.Listen(2);

    using var user0 = new MasterChannel(socket);
    using var user1 = new MasterChannel(socket);

    Console.WriteLine("Game established.");
    
    var win = new Master(user0, user1).Run();
    
    Console.WriteLine($"Game closed. Player {win} win.");
    
    socket.Shutdown(SocketShutdown.Both);
}

return 0;
