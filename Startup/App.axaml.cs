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
    private readonly AssetScanner _assetScanner;
    private readonly StrongReferenceMessenger _messenger = StrongReferenceMessenger.Default;

    public App()
    {
        var dispatcher = Dispatcher.UIThread;
        _assetScanner = new AssetScanner(_messenger, dispatcher);
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
        _assetScanner.Start();
    }

    private void HandleImageScannerStopRequest(object recipient, ImageScannerStopRequestMessage message)
    {
        _assetScanner.RequestStop();
        _assetScanner.Join();
    }

    private void HandleDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        _messenger.UnregisterAll(this);
    }
}