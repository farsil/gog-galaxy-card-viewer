using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindow : Window
{
    private const int LoadingThresholdPixels = 100;

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

    private void HandleScrollViewerScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;

        var distanceFromBottom = ScrollViewer.Extent.Height - ScrollViewer.Viewport.Height - ScrollViewer.Offset.Y;

        if (distanceFromBottom < LoadingThresholdPixels && ScrollViewer.Offset.Y > 0)
            vm.LoadMoreSearchResults();
    }

    private void HandleSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ScrollViewer.Offset.Y > 0)
            ScrollViewer.ScrollToHome();
    }
}