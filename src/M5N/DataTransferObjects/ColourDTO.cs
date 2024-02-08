using M5N.Primitives;

namespace M5N.DataTransferObjects;

public record struct ColourDTO(Colour Colour) : IChannelObject<ColourDTO>
{
    public static TagCode TagCode { get; } = TagCode.Colour;
}