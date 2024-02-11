using System.Dynamic;

namespace M5N.Interop;

public abstract class InteropModule : DynamicObject
{
    public virtual INameResolvingPolicy NameResolvingPolicy { get; set; } = DirectNameResolvingPolicy.Default;
    
    public abstract override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result);
}
