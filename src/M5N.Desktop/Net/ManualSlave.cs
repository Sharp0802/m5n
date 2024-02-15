using System.Windows;
using M5N.Logging;
using M5N.Primitives;
using Microsoft.Extensions.Logging;

namespace M5N.Desktop.Net;

public class ManualSlave(MapDisplay display) : DesktopSlave(display)
{
    private readonly MapDisplay _display = display;

    public override Colour InquiryColour()
    {
        Log.CallerMember(this);
        
        var result = MessageBox.Show(
            $"""
            You currently has {Enum.GetName(Colour)} stone.
            But, you can swap your colour with other player. (Yes)
            """,
            "Swap colour?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        
        return result == MessageBoxResult.No
            ? Colour
            : Colour is Colour.Black 
                ? Colour.White 
                : Colour.Black;
    }

    public override (byte X, byte Y) InquiryStone()
    {
        Log.CallerMember(this);
        
        _display.Dispatcher.Invoke(() => _display.IsWatermarkVisible = false);

        var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        MapDisplayEventArgs    args = null!;
        MapDisplayEventHandler handler = null!;
        handler = (_, e) =>
        {
            args = e;
            // ReSharper disable once AccessToDisposedClosure
            waitHandle.Set();
            _display.Placed -= handler;
            return Task.CompletedTask;
        };
        _display.Placed += handler;

        waitHandle.WaitOne();
        waitHandle.Dispose();
        
        _display.Dispatcher.Invoke(() => _display.IsWatermarkVisible = true);

        return ((byte, byte))(args.X, args.Y);
    }

    public override TagCode MakeDecision()
    {
        Log.CallerMember(this);
        
        var result = MessageBox.Show(
            """
            You can choose your colour (Yes)
            -or-
            pass a chance of choosing colour to the other player
            and place two more stone. (No)
            """,
            "Choose colour?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        
        Logger.LogInformation("MakeDecision ends~!");
        
        return result == MessageBoxResult.Yes 
            ? TagCode.Colour 
            : TagCode.Coordinate;
        
    }
}