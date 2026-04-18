namespace GogGalaxyCardViewer.Scan;

public sealed class VerticalCoverFoundMessage(VerticalCover verticalCover)
{
    public VerticalCover VerticalCover => verticalCover;
}

public sealed class ImageScannerStartRequestMessage;

public sealed class ImageScannerStopRequestMessage;