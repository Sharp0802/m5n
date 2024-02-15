
using System.Net;
using System.Net.Sockets;
using M5N.Master;
using M5N.Primitives;

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

    // Colour will be swapped/inverted.
    using var user0 = new MasterChannel(socket, Colour.White);
    using var user1 = new MasterChannel(socket, Colour.Black);

    Console.WriteLine("Game established.");
    
    var win = new Master(user0, user1).Run();
    
    Console.WriteLine($"Game closed. Player {win} win.");
    Console.ReadKey();
}

return 0;
