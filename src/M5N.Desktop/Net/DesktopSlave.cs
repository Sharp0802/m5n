using System.Windows.Media;
using M5N.Logging;
using M5N.Primitives;
using M5N.Slave.Shared;
using Microsoft.Extensions.Logging;
using LoggerFactory = M5N.Logging.LoggerFactory;

namespace M5N.Desktop.Net;

public abstract class DesktopSlave(MapDisplay display) : ISlave, ITraceable<DesktopSlave>
{
    public ILogger<DesktopSlave> Logger { get; } = LoggerFactory.Acquire<DesktopSlave>();

    public Colour Colour
    {
        get => display.Colour == Colors.Black ? Colour.Black : Colour.White;
        set
        {
            Log.CallerMember(this);
            display.Colour = value is Colour.Black ? Colors.Black : Colors.White;
        }
    }

    protected float[] Weights { get; } = new float[15 * 15];

    public abstract Colour           InquiryColour();
    public abstract (byte X, byte Y) InquiryStone();
    public abstract TagCode          MakeDecision();

    private void UpdateWeight()
    {
        throw new NotImplementedException();
    }
    
    public void SetStone(byte x, byte y, Colour colour)
    {
        Log.CallerMember(this);
        
        UpdateWeight();
        display.Update(Weights);
        
        display.Dispatcher.Invoke(() =>
        {
            display.Place(x, y).Fill = new SolidColorBrush(
                colour is Colour.Black
                    ? Colors.Black
                    : Colors.White);
        });
        
        Logger.LogInformation("SetStone end~!");
    }

    public void DeclareVictory()
    {
        Log.CallerMember(this);
        
        display.Dispatcher.Invoke(() =>
        {
            display.IsWatermarkVisible = true;
            display.WatermarkText      = "Victory!";
        });
    }

    public void DeclareDefeat()
    {
        Log.CallerMember(this);
        
        display.Dispatcher.Invoke(() =>
        {
            display.IsWatermarkVisible = true;
            display.WatermarkText      = "Defeated...";
        });
    }
}