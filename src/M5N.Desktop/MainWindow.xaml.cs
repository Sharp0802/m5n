using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using M5N.Desktop.Net;
using M5N.Logging;
using M5N.Slave.Shared;
using Microsoft.Extensions.Logging;
using LoggerFactory = M5N.Logging.LoggerFactory;

namespace M5N.Desktop;

public partial class MainWindow : ITraceable<MainWindow>
{
    public ILogger<MainWindow> Logger { get; } = LoggerFactory.Acquire<MainWindow>();

    private CancellationTokenSource _cts = new();
    private Task?                   _task;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void AttachDebugger(object sender, RoutedEventArgs e)
    {
        Log.CallerMember(this);

        if (!Debugger.IsAttached && !Debugger.Launch())
        {
            MessageBox.Show("Couldn't attach debugger",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            return;
        }

        if (Debugger.IsAttached)
            ((Button)sender).IsEnabled = false;
    }

    private void ViewVersion(object sender, RoutedEventArgs e)
    {
        Log.CallerMember(this);

        MessageBox.Show(
            this,
            $"""
             M5N.Desktop v{typeof(MainWindow).Assembly.GetName().Version}
             Running on .NET {RuntimeEnvironment.GetSystemVersion()}
             """,
            "About",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }


    private bool _isRunning;


    private bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;

            StartButton.IsEnabled   = !value;
            ConnectButton.IsEnabled = !value;
            HaltButton.IsEnabled    = value;
            Display.IsEnabled       = value;
        }
    }


    private void RunChannel(ISlave policy)
    {
        Log.CallerMember(this);
        
        string   epStr;
        EndPoint ep;
        while (!TryParseEndPoint(epStr = EndPointWindow.Inquiry(), out ep!))
        {
            MessageBox.Show(
                this,
                $"'{epStr}' is not valid endpoint", 
                "Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }

        try
        {
            var channel = new SlaveChannel(ep, policy);
            Logger.LogInformation("Starting slave-channel...");
            _task = channel.RunAsync(_cts.Token);
            
            IsRunning = true;
        }
        catch (SocketException exception)
        {
            Logger.LogCritical(exception, "Couldn't establish connection with '{}'", epStr);
            
            MessageBox.Show(
                $"Connection refused from '{epStr}'", 
                "Error", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error);
        }

        return;
        
        
        static bool TryParseEndPoint(string src, [NotNullWhen(true)] out EndPoint? ep)
        {
            if (IPEndPoint.TryParse(src, out var ipEp))
            {
                ep = ipEp;
                return true;
            }

            var trunc = src.Split(':');
            if (trunc.Length != 2)
            {
                ep = null;
                return false;
            }

            if (!ushort.TryParse(trunc[1], out var port))
            {
                ep = null;
                return false;
            }

            ep = new DnsEndPoint(trunc[0], port);

            return true;
        }
    }

    private void ManualPlay(object sender, RoutedEventArgs e)
    {
        Log.CallerMember(this);
        RunChannel(new ManualSlave(this, Display));
    }

    private void AutoPlay(object sender, RoutedEventArgs e)
    {
        Log.CallerMember(this);
        RunChannel(new AutomaticSlave(Display));
    }

    private async void ForceHalt(object sender, RoutedEventArgs e)
    {
        Log.CallerMember(this);
        Logger.LogInformation("Halting...");
        await _cts.CancelAsync();
        if (_task is not null)
        {
            try
            {
                await _task;
            }
            catch (TaskCanceledException)
            {
                Logger.LogTrace("Task was canceled successfully.");
            }
        }
        Dispatcher.Invoke(() => IsRunning = false);
        _cts.TryReset();
    }
}