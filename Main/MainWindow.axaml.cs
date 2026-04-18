using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is MainWindowViewModel vm) vm.IsActive = true;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (DataContext is MainWindowViewModel vm) vm.IsActive = false;
    }

    private void HandleVendorButtonClick(object? sender, RoutedEventArgs e)
    {
        ScrollViewer.ScrollToHome();
    }
}