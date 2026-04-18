using System;
using System.IO;
using System.Threading;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Data.Sqlite;

namespace GogGalaxyCardViewer.Scan;

public class AssetScanner
{
    private const int VerticalCoverTypeId = 3;
    private const int TitleTypeId = 15;

    private readonly string _databasePath;
    private readonly IDispatcher _dispatcher;
    private readonly IMessenger _messenger;
    private readonly Thread _thread;
    private readonly string _webCachePath;
    private volatile bool _shouldStop;

    public AssetScanner(IMessenger messenger, IDispatcher dispatcher)
    {
        _webCachePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GOG.com", "Galaxy", "webcache"
        );
        _databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GOG.com", "Galaxy", "storage", "galaxy-2.0.db"
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
        if (!Directory.Exists(_webCachePath) || !File.Exists(_databasePath)) return;

        using var connection = new SqliteConnection($"Data Source={_databasePath}");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                              select
                                u.id as userId,
                                json_extract(gp.value, "$.title") as title,
                                wcr.filename as cardFilename,
                                gp.releaseKey as releaseKey
                              from Users u
                              join WebCache wc on u.id = wc.userId
                              join GamePieces gp on gp.releaseKey = wc.releaseKey
                              join WebCacheResources wcr on wcr.webCacheId = wc.id
                              where wcr.webCacheResourceTypeId = $resourceTypeId and gp.gamePieceTypeId = $gamePieceTypeId
                              """;
        command.Parameters.AddWithValue("$resourceTypeId", VerticalCoverTypeId);
        command.Parameters.AddWithValue("$gamePieceTypeId", TitleTypeId);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            if (_shouldStop) break;

            var userId = reader.GetString(0);
            var title = reader.GetString(1);
            var cardFilename = reader.GetString(2);

            var releaseKey = reader.GetString(3).Split("_");
            var vendor = releaseKey[0];
            var vendorId = releaseKey[1];

            var cardPath = Path.Combine(_webCachePath, userId, vendor, vendorId, cardFilename);
            if (!File.Exists(cardPath)) continue;

            var verticalCover = new VerticalCover
            {
                GameTitle = title,
                Vendor = vendor,
                Path = cardPath
            };

            _dispatcher.Post(() => _messenger.Send(new VerticalCoverFoundMessage(verticalCover)));

            Console.WriteLine($"asset-scanner: Vertical cover found at {cardPath}");
        }
    }
}