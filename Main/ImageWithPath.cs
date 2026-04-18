using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace GogGalaxyCardViewer.Main;

public sealed class ImageWithPath : Image
{
    public static readonly StyledProperty<string> PathProperty =
        AvaloniaProperty.Register<ImageWithPath, string>(nameof(Path));

    public string Path
    {
        get => GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PathProperty) Source = new Bitmap(Path);
    }
}