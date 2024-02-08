using System.ComponentModel.DataAnnotations;
using M5N.Primitives;

namespace M5N.DataTransferObjects;

public record struct CoordinateDTO(
    [property: Range(0, 14)] byte X,
    [property: Range(0, 14)] byte Y,
    Colour                        Colour)
    : IChannelObject<CoordinateDTO>
{
    public static TagCode TagCode { get; } = TagCode.Coordinate;
}