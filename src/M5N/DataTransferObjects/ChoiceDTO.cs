using M5N.Primitives;

namespace M5N.DataTransferObjects;

public record struct ChoiceDTO(TagCode Tag) : IChannelObject<ChoiceDTO>
{
    public static TagCode TagCode { get; } = TagCode.Choice;
}