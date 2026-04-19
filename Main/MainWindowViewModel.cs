using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public class MainWindowViewModel(IMessenger messenger, IDispatcher dispatcher, IAssetScanner assetScanner)
    : ObservableRecipient(messenger), IRecipient<VerticalCoverFoundMessage>
{
    private const int SearchResultsMax = 100;

    private readonly List<VerticalCover> _allCovers = [];

    private List<VerticalCover> _allSearchResults = [];

    public ObservableCollection<VerticalCover> SearchResults { get; private set; } = [];

    public int AllSearchResultsCount => _allSearchResults.Count;

    public string? SearchText
    {
        get;
        set
        {
            if (field == value) return;
            field = value;

            var allSearchResults = _allCovers.Where(ShouldBeInResults).ToList();

            dispatcher.Post(() =>
            {
                _allSearchResults = _allCovers.Where(ShouldBeInResults).ToList();
                OnPropertyChanged(nameof(AllSearchResultsCount));

                SearchResults = new ObservableCollection<VerticalCover>(_allSearchResults.Take(SearchResultsMax));
                OnPropertyChanged(nameof(SearchResults));
            }, DispatcherPriority.Background);
        }
    }

    public void Receive(VerticalCoverFoundMessage message)
    {
        _allCovers.Add(message.VerticalCover);

        if (ShouldBeInResults(message.VerticalCover))
        {
            if (SearchResults.Count < SearchResultsMax)
                SearchResults.Add(message.VerticalCover);

            _allSearchResults.Add(message.VerticalCover);
            OnPropertyChanged(nameof(AllSearchResultsCount));
        }
    }

    private bool ShouldBeInResults(VerticalCover cover)
    {
        return SearchText == null ||
               cover.GameTitle.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase);
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