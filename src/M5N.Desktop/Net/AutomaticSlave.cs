using M5N.Primitives;

namespace M5N.Desktop.Net;

public class AutomaticSlave(MapDisplay display) : DesktopSlave(display)
{
    public override Colour InquiryColour()
    {
        // Always select BLACK to simplify codes
        return Colour.Black;
    }

    public override (byte X, byte Y) InquiryStone()
    {
        // TODO
        throw new NotImplementedException();
    }

    public override TagCode MakeDecision()
    {
        // Always select COLOUR to simplify codes
        return TagCode.Colour;
    }
}