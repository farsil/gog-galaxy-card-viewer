using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindowViewModel(IMessenger messenger)
    : ObservableRecipient(messenger), IRecipient<ImageFoundMessage>
{
    private readonly List<string> _allImages = [];
    private string? _currentFilter;

    public ObservableCollection<string> FilteredImages { get; private set; } = [];

    public void Receive(ImageFoundMessage message)
    {
        _allImages.Add(message.Path);

        Dispatcher.UIThread.Post(() =>
        {
            if (_currentFilter == null || message.Path.Contains(_currentFilter))
                FilteredImages.Add(message.Path);
        }, DispatcherPriority.ContextIdle);
    }

    [RelayCommand]
    private void FilterImages(string? filter)
    {
        _currentFilter = filter;

        FilteredImages = _currentFilter == null
            ? new ObservableCollection<string>(_allImages)
            : new ObservableCollection<string>(_allImages.Where(p => p.Contains(_currentFilter)));

        OnPropertyChanged(nameof(FilteredImages));
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        Messenger.Send(new ImageScannerStartRequestMessage());
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        Messenger.Send(new ImageScannerStopRequestMessage());
    }
}