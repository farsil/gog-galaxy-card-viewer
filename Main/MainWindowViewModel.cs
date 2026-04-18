using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindowViewModel(IMessenger messenger, IAssetScanner assetScanner)
    : ObservableRecipient(messenger), IRecipient<VerticalCoverFoundMessage>
{
    private const int SearchResultsMax = 100;

    private readonly List<VerticalCover> _allCovers = [];

    public ObservableCollection<VerticalCover> SearchResults { get; private set; } = [];

    [ObservableProperty] public partial int PossibleResultsCount { get; set; }

    [ObservableProperty] public partial int ActualResultsCount { get; set; }

    public string? SearchText
    {
        get;
        set
        {
            if (field == value) return;
            field = value;

            var allSearchResults = _allCovers.Where(ShouldBeInResults).ToList();

            SearchResults = new ObservableCollection<VerticalCover>(allSearchResults.Take(SearchResultsMax));
            OnPropertyChanged(nameof(SearchResults));

            PossibleResultsCount = allSearchResults.Count;
            ActualResultsCount = SearchResults.Count;
        }
    }

    public void Receive(VerticalCoverFoundMessage message)
    {
        _allCovers.Add(message.VerticalCover);

        if (ShouldBeInResults(message.VerticalCover) && SearchResults.Count < SearchResultsMax)
            SearchResults.Add(message.VerticalCover);

        PossibleResultsCount = _allCovers.Count;
        ActualResultsCount = SearchResults.Count;
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