using M5N.Primitives;

namespace M5N.DataTransferObjects;

public record struct IdentifierDTO(byte Identifier) : IChannelObject<IdentifierDTO>
{
    public static TagCode TagCode { get; } = TagCode.Identifier;
}
