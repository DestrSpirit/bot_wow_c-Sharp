using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace wintool
{
    public static class MouseCapturer
    {
        public static Bitmap Capture()
        {
                var top = Screen.AllScreens.Min(scr => scr.Bounds.Top);
                var left = Screen.AllScreens.Min(scr => scr.Bounds.Left);
                var bottom = Screen.AllScreens.Max(scr => scr.Bounds.Bottom);
                var right = Screen.AllScreens.Max(scr => scr.Bounds.Right);
                var size = new Size(right - left, bottom - top);
                var bitmap = new Bitmap(size.Width, size.Height);
                Rectangle rectangle = new Rectangle(0, 0, 50, 50);
                using (var graphics = Graphics.FromImage(bitmap))
                {

                    graphics.FillRectangle(Brushes.Black, rectangle);

                    using (var cursor = CursorInfo.Get())
                    {
                        if (cursor==null || cursor.State != CursorState.Showing)
                        {
                        bitmap.Save("save.png");
                            return bitmap;
                        }

                        using (var cursorIcon = cursor.GetIconInfo())
                        {
                            graphics.DrawIcon(
                                cursor.Icon,
                                0,
                                0
                                );
                        }
                    }
                }
                return bitmap;

        }

        #region Interop

        public class CursorInfo : IDisposable
        {
            public CursorState State { get; private set; }
            public Icon Icon { get; private set; }
            public Point ScreenPosition { get; private set; }

            private CursorInfo() { }
            public static CursorInfo Get()
            {

                
                try
                {
                    var result = new CursorInfo();
                    Native.CursorInfo info;
                    if (!Native.GetCursorInfo(out info))
                    {
                        throw new Win32Exception();
                    }

                    result.State = info.cursorState;
                    result.Icon = Icon.FromHandle(info.hCursor);
                    result.ScreenPosition = new Point(info.ptScreenPos.X, info.ptScreenPos.Y);

                    return result;
                } catch
                {
                    return null;
                }


            }

            public IconInfo GetIconInfo()
            {
                return IconInfo.FromIcon(this.Icon);
            }

            bool isDisposed = false;
            public void Dispose()
            {
                if (!isDisposed)
                {
                        Icon.Dispose();
                }
                isDisposed = true;
            }
        }

        public enum CursorState
        {
            Hidden,
            Showing,
            Suppressed
        }

        public class IconInfo : IDisposable
        {
            private IconInfo(Native.IconInfo nativeIconInfo)
            {
                xHotspot = nativeIconInfo.xHotspot;
                yHotspot = nativeIconInfo.yHotspot;
                IsCursor = !nativeIconInfo.fIcon;
                if (nativeIconInfo.hbmColor != IntPtr.Zero)
                {
                    Color = Bitmap.FromHbitmap(nativeIconInfo.hbmColor);
                }
                if (nativeIconInfo.hbmMask != IntPtr.Zero)
                {
                    Mask = Bitmap.FromHbitmap(nativeIconInfo.hbmMask);
                }
            }

            public int xHotspot { get; private set; }
            public int yHotspot { get; private set; }
            public bool IsCursor { get; private set; }
            public Bitmap Color { get; private set; }
            public Bitmap Mask { get; private set; }

            public static IconInfo FromIcon(Icon icon)
            {
                if (icon == null)
                {
                    throw new ArgumentNullException("icon");
                }
                return IconInfo.FromHIcon(icon.Handle);
            }

            public static IconInfo FromHIcon(IntPtr hIcon)
            {
                Native.IconInfo nativeIconInfo;
                if (!Native.GetIconInfo(hIcon, out nativeIconInfo))
                {
                    throw new Win32Exception();
                }
                return new IconInfo(nativeIconInfo);
            }

            bool isDisposed = false;
            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                if (Mask != null)
                {
                    Mask.Dispose();
                }
                if (Color != null)
                {
                    Color.Dispose();
                }

                isDisposed = true;
            }
        }

        static class Native
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public Int32 X;
                public Int32 Y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CursorInfo
            {
                /// <remarks>
                /// The caller must set this to Marshal.SizeOf(typeof(CursorInfo)).
                /// </remarks>
                public Int32 cbSize;
                public CursorState cursorState;
                public IntPtr hCursor;
                public Point ptScreenPos;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct IconInfo
            {
                /// <summary>
                /// Specifies whether this structure defines an icon or a cursor.
                /// true = icon, false = cursor
                /// </summary>
                public bool fIcon;
                public Int32 xHotspot;
                public Int32 yHotspot;
                public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
                // this bitmask is formatted so that the upper half is the icon AND bitmask and the lower half is 
                // the icon XOR bitmask. Under this condition, the height should be an even multiple of two. If 
                // this structure defines a color icon, this mask only defines the AND bitmask of the icon. 
                public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
                // structure defines a black and white icon. The AND bitmask of hbmMask is applied with the SRCAND 
                // flag to the destination; subsequently, the color bitmap is applied (using XOR) to the 
                // destination by using the SRCINVERT flag. 
            }

            public static bool GetCursorInfo(out CursorInfo cursorInfo)
            {
                cursorInfo.cbSize = Marshal.SizeOf(typeof(CursorInfo));
                return GetCursorInfoNative(out cursorInfo);
            }

            [DllImport("user32.dll", SetLastError = false, EntryPoint = "GetCursorInfo")]
            static extern bool GetCursorInfoNative(out CursorInfo cursorInfo);


            [DllImport("user32.dll", SetLastError = false)]
            public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo iconInfo);
        }

        #endregion
    }
}