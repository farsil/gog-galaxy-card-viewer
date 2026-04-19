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
    private const int SearchResultsDefaultCount = 100;
    private const int SearchResultsCountIncrement = 25;

    private readonly List<VerticalCover> _allCovers = [];

    private List<VerticalCover> _allSearchResults = [];

    private int _searchResultsMax = SearchResultsDefaultCount;

    public ObservableCollection<VerticalCover> SearchResults { get; private set; } = [];

    public int AllSearchResultsCount => _allSearchResults.Count;

    public string? SearchText
    {
        get;
        set
        {
            if (field == value) return;
            field = value;

            dispatcher.Post(() =>
            {
                _allSearchResults = _allCovers.Where(ShouldBeInResults).ToList();
                OnPropertyChanged(nameof(AllSearchResultsCount));

                _searchResultsMax = SearchResultsDefaultCount;
                RebuildSearchResults();
            }, DispatcherPriority.Background);
        }
    }

    public void Receive(VerticalCoverFoundMessage message)
    {
        _allCovers.Add(message.VerticalCover);

        if (ShouldBeInResults(message.VerticalCover))
        {
            if (SearchResults.Count < _searchResultsMax)
                SearchResults.Add(message.VerticalCover);

            _allSearchResults.Add(message.VerticalCover);
            OnPropertyChanged(nameof(AllSearchResultsCount));
        }
    }

    public void LoadMoreSearchResults()
    {
        dispatcher.Post(() =>
        {
            _searchResultsMax += SearchResultsCountIncrement;
            RebuildSearchResults();
        }, DispatcherPriority.Background);
    }

    private void RebuildSearchResults()
    {
        SearchResults = new ObservableCollection<VerticalCover>(_allSearchResults.Take(_searchResultsMax));
        OnPropertyChanged(nameof(SearchResults));
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