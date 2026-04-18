using Avalonia;
using Avalonia.Controls;

namespace GogGalaxyCardViewer.Main;

public sealed class ResultsText : TextBlock
{
    public static readonly StyledProperty<int> CurrentCountProperty =
        AvaloniaProperty.Register<ResultsText, int>(nameof(CurrentCount));

    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<ResultsText, int>(nameof(MaxCount));

    public ResultsText()
    {
        Text = "Showing the first 0 results out of 0 total";
    }

    public int CurrentCount
    {
        get => GetValue(CurrentCountProperty);
        set => SetValue(CurrentCountProperty, value);
    }

    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CurrentCountProperty || change.Property == MaxCountProperty)
            Text = $"Showing the first {CurrentCount} results out of {MaxCount} total";
    }
}