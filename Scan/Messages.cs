namespace GogGalaxyCardViewer.Scan;

public sealed class ImageFoundMessage(string path)
{
    public string Path => path;
}

public sealed class ImageScannerStartRequestMessage;

public sealed class ImageScannerStopRequestMessage;