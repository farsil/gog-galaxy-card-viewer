using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;

namespace GogGalaxyCardViewer.Main;

public sealed class ImageWithPath : Image
{
    public static readonly StyledProperty<string> PathProperty =
        AvaloniaProperty.Register<ImageWithPath, string>(nameof(Path));

    private static readonly Dictionary<string, Bitmap> Cache = [];

    public string Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PathProperty)
        {
            if (!Cache.ContainsKey(Path))
                Cache[Path] = new Bitmap(Path);
            Source = Cache[Path];
        }
    }

    protected override async void OnPointerPressed(PointerPressedEventArgs e)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) throw new InvalidOperationException("TopLevel is null");

            var dragData = new DataTransfer();
            var item = new DataTransferItem();
            item.SetFile(await topLevel.StorageProvider.TryGetFileFromPathAsync(new Uri(Path)));
            dragData.Add(item);

            await DragDrop.DoDragDropAsync(e, dragData, DragDropEffects.Copy);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to perform drag and drop: " + ex.Message);
        }
    }
}