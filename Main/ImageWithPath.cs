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

    private static readonly BitmapCache Cache = new();

    public string Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == PathProperty)
            Source = e.NewValue is string newValue ? Cache.Get(newValue) : null;
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

    private sealed class BitmapCache
    {
        private readonly Dictionary<string, WeakReference<Bitmap>> _bitmaps = [];

        public Bitmap Get(string path)
        {
            if (_bitmaps.TryGetValue(path, out var reference) && reference.TryGetTarget(out var bitmap))
                return bitmap;

            var newBitmap = new Bitmap(path);
            _bitmaps[path] = new WeakReference<Bitmap>(newBitmap);

            return newBitmap;
        }
    }
}