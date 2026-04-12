using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public partial class MainWindowViewModel(IMessenger messenger)
    : ObservableRecipient(messenger), IRecipient<ImageFoundMessage>
{
    private const int ReceiveThreshold = 200;
    private const int LoadAmount = 50;

    private readonly List<string> _allImages = [];

    public ObservableCollection<string> LoadedImages { get; } = [];


    public void Receive(ImageFoundMessage message)
    {
        _allImages.Add(message.Path);

        if (LoadedImages.Count < ReceiveThreshold)
            LoadedImages.Add(message.Path);
    }

    [RelayCommand]
    private void FilterImages(string? filter)
    {
    }

    public void LoadMoreItems()
    {
        var limit = Math.Min(_allImages.Count, LoadedImages.Count + LoadAmount);

        for (var i = LoadedImages.Count; i < limit; i++)
            LoadedImages.Add(_allImages[i]);
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