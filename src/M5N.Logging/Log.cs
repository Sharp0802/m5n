using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace M5N.Logging;

public class Log
{
    public static void CallerMember<T>(
        ITraceable<T> caller, 
        [CallerMemberName] string? member = null) 
        where T : ITraceable<T>
    {
        caller.Logger.LogTrace("{}.{} has called", typeof(T).Name, member);
    }
}
