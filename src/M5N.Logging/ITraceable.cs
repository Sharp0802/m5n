using Microsoft.Extensions.Logging;

namespace M5N.Logging;

public interface ITraceable<out T> where T : ITraceable<T>
{
    public ILogger<T> Logger { get; }
}
