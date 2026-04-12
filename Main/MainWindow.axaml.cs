using System;
using Avalonia.Controls;
using Avalonia.Input;
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

    private void HandleScrollViewerScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;
        if (DataContext is not MainWindowViewModel vm) return;

        var distanceFromBottom = sv.Extent.Height - sv.Viewport.Height - sv.Offset.Y;
        if (distanceFromBottom < 100) vm.LoadMoreItems();
    }

    private async void HandleImagePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        try
        {
            if (sender is not ImageWithPath image) return;

            var dragData = new DataTransfer();
            var item = new DataTransferItem();
            item.SetFile(await StorageProvider.TryGetFileFromPathAsync(new Uri(image.Path)));
            dragData.Add(item);

            await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Copy);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to initiate drag and drop: " + ex.Message);
        }
    }
}