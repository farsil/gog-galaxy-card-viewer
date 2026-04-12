using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace GogGalaxyCardViewer.Main;

public sealed class PathToImageConverter : IValueConverter
{
    public static readonly PathToImageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;

        if (value is not string path || !targetType.IsAssignableTo(typeof(IImage)))
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        if (path == string.Empty) return null;

        try
        {
            return new Bitmap(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"image-converter: Unable to load image at {path}: {ex.Message}");
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}