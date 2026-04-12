using Avalonia;
using Avalonia.Controls;

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
}