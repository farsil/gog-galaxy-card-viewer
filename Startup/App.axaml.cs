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
    private static readonly StrongReferenceMessenger Messenger = StrongReferenceMessenger.Default;
    private static readonly Dispatcher Dispatcher = Dispatcher.UIThread;

    private static MainWindow CreateWindow()
    {
        return new MainWindow
        {
            DataContext = new MainWindowViewModel(
                Messenger,
                Dispatcher,
                new AssetScanner(Messenger, Dispatcher)
            )
        };
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = CreateWindow();

        base.OnFrameworkInitializationCompleted();
    }
}