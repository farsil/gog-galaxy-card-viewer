using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindowViewModel(IMessenger messenger, IDispatcher dispatcher, IAssetScanner assetScanner)
    : ObservableRecipient(messenger), IRecipient<VerticalCoverFoundMessage>
{
    private readonly List<VerticalCover> _allCovers = [];
    private string? _currentVendor;

    public ObservableCollection<VerticalCover> FilteredCovers { get; private set; } = [];

    public void Receive(VerticalCoverFoundMessage message)
    {
        _allCovers.Add(message.VerticalCover);

        dispatcher.Post(() =>
        {
            if (_currentVendor == null || message.VerticalCover.Vendor == _currentVendor)
                FilteredCovers.Add(message.VerticalCover);
        }, DispatcherPriority.ContextIdle);
    }

    [RelayCommand]
    private void FilterImages(string? filter)
    {
        _currentVendor = filter;

        FilteredCovers = _currentVendor == null
            ? new ObservableCollection<VerticalCover>(_allCovers)
            : new ObservableCollection<VerticalCover>(_allCovers.Where(p => p.Vendor == _currentVendor));

        OnPropertyChanged(nameof(FilteredCovers));
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        assetScanner.Start();
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        assetScanner.RequestStop();
        assetScanner.Join();
    }
}