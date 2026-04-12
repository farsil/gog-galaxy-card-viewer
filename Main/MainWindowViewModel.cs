using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public class MainWindowViewModel(IMessenger messenger) : ObservableRecipient(messenger)
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    protected override void OnActivated()
    {
        base.OnActivated();

        Messenger.Send(new ImageScannerStartRequestMessage());
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        Messenger.Send(new ImageScannerStopRequestMessage());
    }
}