using System;
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

    public ObservableCollection<VerticalCover> SearchResults { get; private set; } = [];

    public string? SearchText
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            UpdateSearchResults();
        }
    }

    public void Receive(VerticalCoverFoundMessage message)
    {
        _allCovers.Add(message.VerticalCover);

        dispatcher.Post(() =>
        {
            if (ShouldCoverBeInResults(message.VerticalCover))
                SearchResults.Add(message.VerticalCover);
        }, DispatcherPriority.ContextIdle);
    }

    private bool ShouldCoverBeInResults(VerticalCover cover)
    {
        if (_currentVendor != null && cover.Vendor != _currentVendor) return false;
        return SearchText == null ||
               cover.GameTitle.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase);
    }

    private void UpdateSearchResults()
    {
        SearchResults = new ObservableCollection<VerticalCover>(_allCovers.Where(ShouldCoverBeInResults).Take(100));
        OnPropertyChanged(nameof(SearchResults));
    }

    [RelayCommand]
    private void FilterImagesByVendor(string? vendor)
    {
        if (vendor == _currentVendor) return;
        _currentVendor = vendor;
        UpdateSearchResults();
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