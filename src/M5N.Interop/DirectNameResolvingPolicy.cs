namespace M5N.Interop;

public class DirectNameResolvingPolicy : INameResolvingPolicy
{
    public static INameResolvingPolicy Default => new DirectNameResolvingPolicy();
    
    public string Resolve(string source) => source;
}
