namespace M5N.Interop;

public interface IInteropEngine : IDisposable
{
    public string Name { get; }
    
    public dynamic ImportModule(string name);
}