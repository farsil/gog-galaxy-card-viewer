using CommunityToolkit.Mvvm.ComponentModel;

namespace GogGalaxyCardViewer.Main;

public class MainWindowViewModel : ObservableObject
{
    public string Greeting { get; } = "Welcome to Avalonia!";
}