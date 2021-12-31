using OverlayLib;

namespace _WinFormsDEMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            IOverlay overlay = new Overlay(Handle);
            overlay.Opacity = 128;
            overlay.IsCaptionEnabled = false;
            
            this.Size = new Size(900, 900);
            overlay.IsTransparent = true;
            overlay.IsCaptionEnabled = true;
        }
    }
} 