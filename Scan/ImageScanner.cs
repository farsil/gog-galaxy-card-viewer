using System;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;

namespace GogGalaxyCardViewer.Scan;

public class ImageScanner
{
    private readonly IDispatcher _dispatcher;
    private readonly IMessenger _messenger;
    private readonly Thread _thread;
    private readonly string _webcachePath;
    private volatile bool _shouldStop;

    public ImageScanner(IMessenger messenger, IDispatcher dispatcher)
    {
        _webcachePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GOG.com", "Galaxy", "webcache"
        );

        _messenger = messenger;
        _dispatcher = dispatcher;

        _shouldStop = false;
        _thread = new Thread(Run);
    }

    public void Start()
    {
        _thread.Start();
    }

    public void Join()
    {
        _thread.Join();
    }

    public void RequestStop()
    {
        _shouldStop = true;
    }

    private void Run()
    {
        if (!Directory.Exists(_webcachePath)) return;

        var gogCachePath = Directory
            .EnumerateDirectories(_webcachePath, "gog", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (gogCachePath == null) return;

        var baseCachePath = Path.GetFullPath(Path.Combine(gogCachePath, ".."));

        foreach (var filename in Directory.EnumerateFiles(baseCachePath, "*glx_vertical_cover*",
                     SearchOption.AllDirectories))
        {
            if (_shouldStop) break;

            _dispatcher.Post(() => _messenger.Send(new ImageFoundMessage(filename)));

            Console.WriteLine($"image-scanner: Image found at {filename}");
        }
    }
}