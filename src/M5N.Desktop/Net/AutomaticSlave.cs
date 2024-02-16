using M5N.Logging;
using M5N.Primitives;
using Microsoft.Extensions.Logging;

namespace M5N.Desktop.Net;

public class AutomaticSlave(MapDisplay display) : DesktopSlave(display)
{
    public override Colour InquiryColour()
    {
        Log.CallerMember(this);
        
        // Always select BLACK to simplify codes
        return Colour.Black;
    }

    public override (byte X, byte Y) InquiryStone()
    {
        Log.CallerMember(this);

        var i = Weights
                .Select((w, i) => (w, i))
                .Where(t => PlaceMap[t.i] == Colour.None)
                .MaxBy(t => t.w).i;
        
        Logger.LogInformation("Select ({},{})", i % 15, i / 15);
        
        return ((byte, byte))(i % 15, i / 15);
    }

    public override TagCode MakeDecision()
    {
        Log.CallerMember(this);
        
        // Always select COLOUR to simplify codes
        return TagCode.Colour;
    }
}