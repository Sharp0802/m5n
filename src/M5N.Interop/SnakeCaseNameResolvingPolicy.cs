using System.Text;

namespace M5N.Interop;

public class SnakeCaseNameResolvingPolicy : INameResolvingPolicy
{
    public static INameResolvingPolicy Default { get; } = new SnakeCaseNameResolvingPolicy();

    public string Resolve(string source)
    {
        var builder = new StringBuilder(source.Length * 2);
        for (var i = 0; i < source.Length; ++i)
        {
            if (char.IsUpper(source[i]))
            {
                if (i != 0 && source[i - 1] != '_')
                    builder.Append('_');
                builder.Append(char.ToLowerInvariant(source[i]));
            }
            else
            {
                builder.Append(source[i]);
            }
        }

        return builder.ToString();
    }
}