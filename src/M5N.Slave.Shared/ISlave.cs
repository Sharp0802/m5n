using M5N.Primitives;

namespace M5N.Slave.Shared;

public interface ISlave
{
    public Colour Colour { get; set; }

    public Colour InquiryColour();

    public void SetStone(byte x, byte y, Colour colour);

    public (byte X, byte Y) InquiryStone();

    public TagCode MakeDecision();

    public void DeclareVictory();

    public void DeclareDefeat();
}