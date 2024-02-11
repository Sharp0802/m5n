namespace M5N.Interop;

public interface INameResolvingPolicy
{
    public static abstract INameResolvingPolicy Default { get; }

    public string Resolve(string source);
}