using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using M5N.Logging;
using Microsoft.Extensions.Logging;
using LoggerFactory = M5N.Logging.LoggerFactory;

namespace M5N.Desktop;

public class MapDisplayEventArgs(int x, int y, Color color) : EventArgs
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public Color Color { get; } = color;
}

public delegate Task MapDisplayEventHandler(MapDisplay sender, MapDisplayEventArgs args);

public partial class MapDisplay : ITraceable<MapDisplay>
{
    private static readonly SolidColorBrush Transparent = new(Colors.Transparent);
    private static readonly SolidColorBrush Acrylic     = new(Color.FromArgb(100, 255, 255, 255));

    public ILogger<MapDisplay> Logger { get; } = LoggerFactory.Acquire<MapDisplay>();

    private          byte        _colour = 255;
    private readonly byte[]      _map    = new byte[15 * 15];
    private readonly TextBlock[] _texts  = new TextBlock[15 * 15];

    public Color Colour
    {
        get => Color.FromRgb(_colour, _colour, _colour);
        set
        {
            if (value.R != value.G ||
                value.R != value.B ||
                (value.R != 255 && value.R != 0) ||
                value.A != 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Colour value must be white or black");
            }

            _colour = value.R;
        }
    }

    public MapDisplay()
    {
        InitializeComponent();

        Placed += (_, args) =>
        {
            Logger.LogTrace("Display ({},{}) = {}", args.X, args.Y, args.Color);
            return Task.CompletedTask;
        };

        for (var y = 0; y < 15; ++y)
        for (var x = 0; x < 15; ++x)
        {
            (_texts[y * 15 + x] = Label(x, y, 0.0f.ToString("F2"))).Foreground = new SolidColorBrush(Colors.Red);

            var (x1, y1) = (x, y);
            var ellipse = Place(x, y);
            ellipse.MouseLeave += (_, _) => { ellipse.Fill = Transparent; };
            ellipse.MouseEnter += (_, _) => { ellipse.Fill = Acrylic; };
            ellipse.MouseLeftButtonDown += async (_, _) =>
            {
                var colour = Color.FromRgb(_colour, _colour, _colour);

                _map[y1 * 15 + x1] = _colour;
                await Placed.Invoke(this, new MapDisplayEventArgs(x1, y1, colour));
                Place(x1, y1).Fill = new SolidColorBrush(colour);
            };
        }
    }


    public MapDisplayEventHandler? Placed;


    public bool IsWatermarkVisible
    {
        get => Cover.Visibility == Visibility.Visible;
        set => Cover.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
    }

    public string WatermarkText
    {
        get => Watermark.Text;
        set => Watermark.Text = value;
    }

    public Ellipse Place(int x, int y)
    {
        var ellipse = new Ellipse();
        ellipse.Fill = Transparent;
        Grid.SetColumn(ellipse, x);
        Grid.SetRow(ellipse, y);
        GridMap.Children.Add(ellipse);

        return ellipse;
    }

    private TextBlock Label(int x, int y, string text)
    {
        var block = new TextBlock();
        block.Text       = text;
        block.Foreground = Brushes.Magenta;
        Grid.SetColumn(block, x);
        Grid.SetRow(block, y);
        GridMap.Children.Add(block);
        return block;
    }

    private static byte ColourClamp(float v)
    {
        v *= 255;
        return (byte)short.Clamp((short)v, 0, 255);
    }

    private static byte InvertColour(byte v)
    {
        return (byte)(255 - v);
    }

    public void Update(Span<float> weights)
    {
        for (var y = 0; y < 15; ++y)
        for (var x = 0; x < 15; ++x)
        {
            var text   = _texts[y * 15 + x];
            var weight = weights[y * 15 + x];
            var colour = Color.FromRgb(InvertColour(ColourClamp(weight)), ColourClamp(weight), 0);

            text.Text       = weight.ToString("F2");
            text.Foreground = new SolidColorBrush(colour);
        }
    }
}