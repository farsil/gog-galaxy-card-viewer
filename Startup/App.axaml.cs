using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Main;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Startup;

public sealed class App : Application
{
    private readonly ImageScanner _imageScanner;
    private readonly StrongReferenceMessenger _messenger = StrongReferenceMessenger.Default;

    public App()
    {
        var dispatcher = Dispatcher.UIThread;
        _imageScanner = new ImageScanner(_messenger, dispatcher);
    }

    private MainWindow CreateWindow()
    {
        return new MainWindow
        {
            DataContext = new MainWindowViewModel(_messenger)
        };
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = CreateWindow();
            desktop.Exit += HandleDesktopExit;
        }

        _messenger.Register<ImageScannerStartRequestMessage>(this, HandleImageScannerStartRequest);
        _messenger.Register<ImageScannerStopRequestMessage>(this, HandleImageScannerStopRequest);

        base.OnFrameworkInitializationCompleted();
    }

    private void HandleImageScannerStartRequest(object recipient, ImageScannerStartRequestMessage message)
    {
        _imageScanner.Start();
    }

    private void HandleImageScannerStopRequest(object recipient, ImageScannerStopRequestMessage message)
    {
        _imageScanner.RequestStop();
        _imageScanner.Join();
    }

    private void HandleDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _messenger.UnregisterAll(this);
    }
}