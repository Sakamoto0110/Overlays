namespace OverlayLib
{
    public interface IOverlay
    {
        bool IsPersistent { get; set; }
        bool IsTransparent { get; set; }
        bool IsCaptionEnabled { get; set; }
        byte Opacity { get; set; }
    }
} 