using System.Buffers;
using System.Buffers.Binary;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace M5N;

[Flags]
public enum ControlCode : ushort
{
    Syn    = 0b10_000000_00000000,
    SynAck = Syn | Ack,
    Ack    = 0b01_000000_00000000
}

public enum ErrorCode : ushort
{
    Success,
    Invalid,
    Timeout
}

public enum TagCode : ushort
{
    Colour,
    Coordinate
}

public delegate void StatusRespondDelegate(ErrorCode error);

public sealed class ChannelRespondContext<T> : IDisposable where T : unmanaged, IChannelObject<T>
{
    private readonly StatusRespondDelegate? _callback;
    
    internal ChannelRespondContext(T? result, StatusRespondDelegate? callback)
    {
        Result    = result;
        _callback = callback;
    }
    
    public T? Result { get; }
    
    public ErrorCode Status { private get; set; }
    
    public void Dispose()
    {
        _callback?.Invoke(Status);
    }
}

public abstract class Channel : IDisposable
{
    private readonly bool   _dispose;
    private readonly Socket _socket;

    private Channel(Socket socket, bool dispose)
    {
        _socket  = socket;
        _dispose = dispose;
    }

    protected Channel(EndPoint endpoint) 
        : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), true)
    {
        _socket.Connect(endpoint);
    }

    protected Channel(Socket server)
        : this(server.Accept(), true)
    {
    }
    
    private bool Receive(
        Memory<byte> buffer, 
        ref TimeSpan timeout)
    {
        var remains = buffer.Length;
        do
        {
            var start = DateTime.UtcNow;

            var task = _socket.ReceiveAsync(buffer).AsTask().WaitAsync(timeout);
            ((Task)task)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing)
                .GetAwaiter()
                .GetResult();
            if (!task.IsCompletedSuccessfully)
            {
                if (task.Exception!.InnerException is TimeoutException)
                    return false;
                throw task.Exception;
            }
            
            remains -= task.Result;
            timeout -= DateTime.UtcNow - start;
        } while (remains > 0);

        return true;
    }

    private void Send(Memory<byte> buffer)
    {
        _socket.Send(buffer.Span);
    }

    private void SendCode(ControlCode ctrl, ushort code)
    {
        var buffer = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)((ushort)ctrl | code));
        Send(buffer);
    }

    private bool ReceiveCode(ref TimeSpan timeout, out ControlCode ctrl, out ushort code)
    {
        var buffer = new byte[2];
        if (!Receive(buffer, ref timeout))
        {
            ctrl = 0;
            code = 0;
            return false;
        }
        code =  BinaryPrimitives.ReadUInt16BigEndian(buffer);
        ctrl =  (ControlCode)code & ControlCode.SynAck;
        code &= (ushort)~ControlCode.SynAck;
        return true;
    }
    
    public ChannelRespondContext<T> Request<T>(TimeSpan timeout) where T : unmanaged, IChannelObject<T>
    {
        // SYN
        SendCode(ControlCode.Syn, (ushort)T.TagCode);

        // SYN-ACK
        if (!ReceiveCode(ref timeout, out _, out _))
        {
            SendCode(ControlCode.Syn, (ushort)ErrorCode.Timeout);
            return new ChannelRespondContext<T>(null, null);
        }
        
        T result;
        using (var owner = MemoryPool<byte>.Shared.Rent(Marshal.SizeOf<T>()))
        {
            if (!Receive(owner.Memory, ref timeout))
            {
                SendCode(ControlCode.Syn, (ushort)ErrorCode.Timeout);
                return new ChannelRespondContext<T>(null, null);
            }
            result = MemoryMarshal.Read<T>(owner.Memory.Span);
        }

        // Validation
        var validation = result.Validate().ToArray();
        if (validation.Length != 0)
            throw new AggregateException("Validation failed.", validation.Cast<Exception>());
        
        
        return new ChannelRespondContext<T>(result, SetStatus);

        void SetStatus(ErrorCode error)
        {
            // ACK
            SendCode(ControlCode.Ack, (ushort)error);
        }
    }

    public ErrorCode Respond<T>(T value) where T : unmanaged, IChannelObject<T>
    {
        SendCode(ControlCode.SynAck, (ushort)T.TagCode);
        using (var owner = MemoryPool<byte>.Shared.Rent(Marshal.SizeOf<T>()))
        {
            MemoryMarshal.Write(owner.Memory.Span, value);
            Send(owner.Memory);
        }

        var timeout = Timeout.InfiniteTimeSpan;
        ReceiveCode(ref timeout, out _, out var code);

        return (ErrorCode)code;
    }

    public abstract void Run();
    
    public void Dispose()
    {
        _socket.Shutdown(SocketShutdown.Both);
        if (_dispose)
            _socket.Dispose();
        GC.SuppressFinalize(this);
    }
}
