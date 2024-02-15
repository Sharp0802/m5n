using System.Windows;

namespace M5N.Desktop;

public partial class EndPointWindow
{
    public EndPointWindow()
    {
        InitializeComponent();
    }

    private void Connect(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public static string Inquiry()
    {
        var wnd = new EndPointWindow();
        wnd.ShowDialog();
        return wnd.EndpointBox.Text;
    }
}