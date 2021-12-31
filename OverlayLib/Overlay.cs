using System.Runtime.InteropServices;

namespace OverlayLib
{

    public class Overlay : IOverlay
    {
        internal static class WinAPIWrapper
        {
            #region WinAPI_Prototypes

            [DllImport("user32.dll", EntryPoint = "SetWindowLongA")]
            internal static extern int SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
            internal static extern int SetWindowPos(IntPtr hwnd, IntPtr hwnd_insert_after, int x, int y, int w, int h,
                uint dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
            internal static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

            [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
            internal static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

            #endregion
             
            #region WinAPI_Constants

            internal const uint WS_CAPTION = 0x00C00000;
            internal const uint WS_HSCROLL = 0x00100000;
            internal const uint WS_VSCROLL = 0x00200000;
            internal const uint WS_SYSMENU = 0x00080000;
            internal const uint WS_MAXIMIZE = 0x01000000;

            internal const uint WS_EX_LAYERED = 0x00080000;
            internal const uint WS_EX_TRANSPARENT = 0x00000020;

            internal const uint SWP_NOMOVE = 0x00000002;
            internal const uint SWP_NOSIZE = 0x00000001;
            internal const uint SWP_SHOWWINDOW = 0x00000040;
            internal const uint SWP_FRAMECHANGED = 0x00000020;

            internal const uint LWA_COLORKEY = 0x00000001;
            internal const uint LWA_ALPHA = 0x00000002;

            internal const int GWL_STYLE = -16;
            internal const int GWL_EXSTYLE = -20;

            internal static IntPtr HWND_TOPMOST = new IntPtr(-1);
            internal static IntPtr HWND_TOP = new IntPtr(0);

            private const uint NOCAPTIONFLAGS = (WS_CAPTION | WS_HSCROLL | WS_VSCROLL | WS_SYSMENU);
            #endregion

            internal static void SetOpacity(IntPtr hwnd, byte opacity)
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
                SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
            }
            internal static void SetTransparent(IntPtr hwnd, byte opacity, bool enable)
            {
                SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | (enable ? WS_EX_TRANSPARENT : 0));
                SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
            }
            internal static void SetCaption(IntPtr hwnd, bool enable)
            {
                SetWindowLong(hwnd, GWL_STYLE, enable ? GetWindowLong(hwnd, GWL_STYLE) & ~NOCAPTIONFLAGS & WS_MAXIMIZE :  _dft_style & ~WS_MAXIMIZE);
            }
            internal static void SetPersistent(IntPtr hwnd, bool enable)
            {
                SetWindowPos(hwnd, enable ? HWND_TOPMOST : HWND_TOP, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
            }

        }

        #region Fields

        private static uint _dft_style;
        private IntPtr _hwnd;


        private bool _IsPersistent;
        private bool _IsTransparent;
        private bool _IsCaptionEnabled;
        private byte _Opacity;

        #endregion

        #region Properties

        public bool IsPersistent
        {
            get => _IsPersistent;
            set
            {
                _IsPersistent = value;
                WinAPIWrapper.SetPersistent(_hwnd, _IsPersistent);
            }
        }
        public bool IsTransparent
        {
            get => _IsTransparent;
            set
            {
                _IsTransparent = value;
                WinAPIWrapper.SetTransparent(_hwnd, Opacity, _IsTransparent);
            }
        }
        public bool IsCaptionEnabled
        {
            get => _IsCaptionEnabled;
            set
            {
                _IsCaptionEnabled = value;
                WinAPIWrapper.SetCaption(_hwnd, !_IsCaptionEnabled);

            }
        }
        public byte Opacity
        {
            get => _Opacity;
            set
            {
                if (_Opacity != value)
                {
                    _Opacity = value;
                    WinAPIWrapper.SetOpacity(_hwnd, Opacity);
                }
            }
        }

        #endregion

        #region Ctor's

        public Overlay(IntPtr hwnd)
        {
            _hwnd = hwnd;
            _dft_style = WinAPIWrapper.GetWindowLong(_hwnd, WinAPIWrapper.GWL_STYLE);
            Opacity = 255;
        }
        public Overlay(IntPtr hwnd, byte opacity)
        {
            _hwnd = hwnd;
            _dft_style = WinAPIWrapper.GetWindowLong(_hwnd, WinAPIWrapper.GWL_STYLE);
            Opacity = opacity;
        }
        public Overlay(IntPtr hwnd, byte opacity, int flags)
        {
            _hwnd = hwnd;
            _dft_style = WinAPIWrapper.GetWindowLong(_hwnd, WinAPIWrapper.GWL_STYLE);
            Opacity = opacity;
            IsTransparent = (flags & OverlayFlags.TRANSPARENT) == OverlayFlags.TRANSPARENT;
            IsCaptionEnabled = (flags & OverlayFlags.CAPTION) == OverlayFlags.CAPTION;
            IsPersistent = (flags & OverlayFlags.PERSISTENT) == OverlayFlags.PERSISTENT;
        }


        #endregion

        #region Methods

        public static void ApplyOverlayToWindowHandle(IntPtr hwnd, byte opacity, int flags)
        {
            _dft_style = WinAPIWrapper.GetWindowLong(hwnd, WinAPIWrapper.GWL_STYLE);
            WinAPIWrapper.SetPersistent(hwnd, (flags & OverlayFlags.PERSISTENT) == OverlayFlags.PERSISTENT);
            WinAPIWrapper.SetTransparent(hwnd, opacity, (flags & OverlayFlags.TRANSPARENT) == OverlayFlags.TRANSPARENT);
            WinAPIWrapper.SetCaption(hwnd, (flags & OverlayFlags.CAPTION) != OverlayFlags.CAPTION);
        }
        //private static void SetOpacity(IntPtr hwnd, byte opacity)
        //{
        //    SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        //    SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
        //}
        //private static void SetTransparent(IntPtr hwnd, byte opacity, bool enable)
        //{
        //    SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | (enable ? WS_EX_TRANSPARENT : 0));
        //    SetLayeredWindowAttributes(hwnd, 0, opacity, LWA_ALPHA);
        //}
        //public static void SetCaption(IntPtr hwnd, bool enable)
        //{
        //    SetWindowLong(hwnd, GWL_STYLE, enable ? GetWindowLong(hwnd, GWL_STYLE) & ~NOCAPTIONFLAGS & WS_MAXIMIZE : _dft_style & ~WS_MAXIMIZE);
        //}
        //private static void SetPersistent(IntPtr hwnd, bool enable)
        //{
        //    SetWindowPos(hwnd, enable ? HWND_TOPMOST : HWND_TOP, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
        //}

        #endregion
    }

}