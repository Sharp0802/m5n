using System.Runtime.CompilerServices;
using System.Windows.Media;
using M5N.Logging;
using M5N.Primitives;
using M5N.Slave.Shared;
using Microsoft.Extensions.Logging;
using LoggerFactory = M5N.Logging.LoggerFactory;

namespace M5N.Desktop.Net;

public abstract class DesktopSlave : ISlave, ITraceable<DesktopSlave>
{
    public ILogger<DesktopSlave> Logger { get; } = LoggerFactory.Acquire<DesktopSlave>();

    public Colour Colour
    {
        get => _display.Colour == Colors.Black ? Colour.Black : Colour.White;
        set
        {
            Log.CallerMember(this);
            _display.Colour = value is Colour.Black ? Colors.Black : Colors.White;
        }
    }

    protected Colour[] PlaceMap { get; } = new Colour[15 * 15];
    
    private readonly MapDisplay _display;

    protected DesktopSlave(MapDisplay display)
    {
        InitializeWeight();
        
        _display = display;
        _display.Update(Weights);
    }

    protected float[] Weights { get; } = new float[15 * 15];

    public abstract Colour           InquiryColour();
    public abstract (byte X, byte Y) InquiryStone();
    public abstract TagCode          MakeDecision();

    private void InitializeWeight()
    {
        for (var y = 0; y < 15; ++y)
        for (var x = 0; x < 15; ++x)
        {
            var ox = 7 - x;
            var oy = 7 - y;

            Weights[y * 15 + x] = 0.1f - (ox * ox + oy * oy) / 1.414f * 0.001f;
        }
    }

    private void UpdateWeight()
    {
        InitializeWeight();
        for (var y = 0; y < 15; ++y)
        for (var x = 0; x < 15; ++x)
            if (PlaceMap[y * 15 + x] is not Colour.None)
                UpdateWeight(x, y);
    }
    
    private void UpdateWeight(int x, int y)
    {
        CountInRay(x, y, +0, +1);
        CountInRay(x, y, +1, +1);
        CountInRay(x, y, +1, +0);
        CountInRay(x, y, +1, -1);
        CountInRay(x, y, +0, -1);
        CountInRay(x, y, -1, -1);
        CountInRay(x, y, -1, +0);
        CountInRay(x, y, -1, +1);

        return;
        
        void CountInRay(int px, int py, int offsetX, int offsetY)
        {
            var origin = PlaceMap[py * 15 + px];
            var count  = 0;
            px += offsetX;
            py += offsetY;

            var continuous = origin;
            var conCount   = 1;
            
            for (; 
                 px is >= 0 and < 15 && 
                 py is >= 0 and < 15 && 
                 count <= 4 &&
                 PlaceMap[py * 15 + px] != Colour;
                 px += offsetX, 
                 py += offsetY,
                 ++count)
            {
                var dX = px - x;
                var dY = py - y;
                var d  = 1 - MathF.Sqrt(dX * dX + dY * dY) / 9.898f;
                
                if (continuous == PlaceMap[py * 15 + px])
                    conCount++;
                else
                {
                    continuous = PlaceMap[py * 15 + px];
                    conCount   = 1;
                }
                
                if (PlaceMap[py * 15 + px] == origin)
                    continue;
                if (PlaceMap[py * 15 + px] == Colour.None)
                    Weights[py * 15 + px] += 0.1f * conCount + d * 0.3f;
                else
                    break;
            }
        }
    }
    
    public void SetStone(byte x, byte y, Colour colour)
    {
        Log.CallerMember(this);

        PlaceMap[y * 15 + x] = colour;
        
        UpdateWeight();
        
        _display.Dispatcher.Invoke(() =>
        {
            _display.Update(Weights);
            _display.Place(x, y).Fill = new SolidColorBrush(
                colour is Colour.Black
                    ? Colors.Black
                    : Colors.White);
        });
        
        Logger.LogInformation("SetStone end~!");
    }

    public void DeclareVictory()
    {
        Log.CallerMember(this);
        
        _display.Dispatcher.Invoke(() =>
        {
            _display.IsWatermarkVisible = true;
            _display.WatermarkText      = "Victory!";
        });
    }

    public void DeclareDefeat()
    {
        Log.CallerMember(this);
        
        _display.Dispatcher.Invoke(() =>
        {
            _display.IsWatermarkVisible = true;
            _display.WatermarkText      = "Defeated...";
        });
    }
}