using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.VisualTree;

namespace GogGalaxyCardViewer.Main;

public sealed partial class SyntheticTitlebar : UserControl
{
    private const double TitleBarHeight = 40;

    private Window? _window;

    public SyntheticTitlebar()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (OperatingSystem.IsWindows())
        {
            IsVisible = true;

            _window = this.FindAncestorOfType<Window>();
            Debug.Assert(_window != null);

            _window.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
            _window.ExtendClientAreaToDecorationsHint = true;
            _window.ExtendClientAreaTitleBarHeightHint = TitleBarHeight;
            _window.PropertyChanged += HandleWindowStateChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        _window?.PropertyChanged -= HandleWindowStateChanged;
    }

    private void HandleWindowStateChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Window.WindowStateProperty)
            MaximizeToggleButton.Content = _window?.WindowState == WindowState.Maximized ? "🗗" : "🗖";
    }

    private void HandleMinimizeButtonClick(object? sender, RoutedEventArgs e)
    {
        _window?.WindowState = WindowState.Minimized;
    }

    private void HandleMaximizeToggleButtonClick(object? sender, RoutedEventArgs e)
    {
        _window?.WindowState = _window.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void HandleCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        _window?.Close();
    }
}