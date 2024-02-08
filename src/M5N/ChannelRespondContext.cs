namespace M5N;

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