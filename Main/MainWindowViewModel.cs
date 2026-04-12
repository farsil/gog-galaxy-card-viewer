using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GogGalaxyCardViewer.Scan;

namespace GogGalaxyCardViewer.Main;

public class MainWindowViewModel(IMessenger messenger) : ObservableRecipient(messenger), IRecipient<ImageFoundMessage>
{
    private const int ReceiveThreshold = 200;
    private const int LoadAmount = 50;

    private readonly List<string> _allImages = [];
    private int _loadedImages;

    public ObservableCollection<string> Images { get; } = [];

    public void Receive(ImageFoundMessage message)
    {
        _allImages.Add(message.Path);

        if (_loadedImages < ReceiveThreshold)
        {
            Images.Add(message.Path);
            _loadedImages++;
        }
    }

    public void LoadMoreItems()
    {
        var limit = Math.Min(_allImages.Count, _loadedImages + LoadAmount);

        for (var i = _loadedImages; i < limit; i++)
            Images.Add(_allImages[i]);

        _loadedImages = limit;
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