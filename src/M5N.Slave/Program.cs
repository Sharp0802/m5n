using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;using M5N.Interop;
using M5N.Interop.NativeLibrary;
using M5N.Interop.Python;
using M5N.Slave;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding  = Encoding.UTF8;

if (args.Length < 1)
{
    Console.Error.WriteLine("E: Insufficient argument count.");
    return 1;
}

var offset = 0;

if (!TryParseEndPoint(args[offset++], out var ep))
{
    Console.Error.WriteLine("E: Invalid endpoint syntax.");
    return 1;
}

if (!TryLoadEngine(args, ref offset, out var engine))
{
    Console.Error.WriteLine("E: Invalid engine syntax.");
    return 1;
}

using (engine)
{
    if (!TryLoadModule(args, ref offset, engine, out var module))
    {
        Console.Error.WriteLine("E: Invalid module syntax.");
        return 1;
    }
    
    var       slave   = new Slave(module);
    using var channel = new SlaveChannel(ep, slave);
    Console.WriteLine("Connection established.");
    channel.Run();
    Console.WriteLine("Connection closed.");
}

return 0;

static bool TryParseEndPoint(string src, [NotNullWhen(true)] out EndPoint? ep)
{
    if (IPEndPoint.TryParse(src, out var ipEp))
    {
        ep = ipEp;
        return true;
    }

    var trunc = src.Split(':');
    if (trunc.Length != 2)
    {
        ep = null;
        return false;
    }

    if (!ushort.TryParse(trunc[1], out var port))
    {
        ep = null;
        return false;
    }

    ep = new DnsEndPoint(trunc[0], port);

    return true;
}

static bool TryLoadEngine(string[] args, ref int offset, [NotNullWhen(true)] out IInteropEngine? engine)
{
    if (args.Length - offset < 1)
    {
        engine = null;
        return false;
    }
    
    switch (args[offset++])
    {
        case "Python":
            if (args.Length - offset < 1)
                goto default;
            engine = new PythonInteropEngine(args[offset++]);
            return true;
        case "C":
            engine = new CInteropEngine();
            return true;
        default:
            engine = null;
            return false;
    }
}

static bool TryLoadModule(
    string[] args, 
    ref int offset, 
    IInteropEngine engine, 
    [NotNullWhen(true)] out InteropModule? module)
{
    if (args.Length - offset < 1)
    {
        module = null;
        return false;
    }

    var name = args[offset++];
    module = engine.ImportModule(name);
    
    return true;
}
