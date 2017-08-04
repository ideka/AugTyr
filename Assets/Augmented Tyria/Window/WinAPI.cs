using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class WinAPI
{
    public static readonly IntPtr Active = GetActiveWindow();

    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    public const int GWL_EX_STYLE = -20;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TRANSPARENT = 0x20;

    public const uint LWA_COLORKEY = 1;
    public const uint LWA_ALPHA = 2;

    public const uint SWP_NOSIZE = 0x1;
    public const uint SWP_FRAMECHANGED = 0x20;
    public const uint SWP_SHOWWINDOW = 0x40;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int X { get { return this.left; } }
        public int Y { get { return this.top; } }
        public int Width { get { return this.right - this.left; } }
        public int Height { get { return this.bottom - this.top; } }

        public Point TopLeft { get { return new Point(left, top); } }
        public Point BottomRight { get { return new Point(right, bottom); } }

        public RECT ClientToScreen(IntPtr hWnd)
        {
            return this.PassPoints(hWnd, p =>
            {
                WinAPI.ClientToScreen(hWnd, ref p);
                return p;
            });
        }

        public RECT ScreenToClient(IntPtr hWnd)
        {
            return this.PassPoints(hWnd, p =>
            {
                WinAPI.ScreenToClient(hWnd, ref p);
                return p;
            });
        }

        private RECT PassPoints(IntPtr hWnd, Func<Point, Point> func)
        {
            Point topLeft = func(this.TopLeft);
            Point bottomRight = func(this.BottomRight);
            return new RECT()
            {
                left = topLeft.X,
                top = topLeft.Y,
                right = bottomRight.X,
                bottom = bottomRight.Y
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COLORREF
    {
        public byte r;
        public byte g;
        public byte b;
        public readonly byte padding;

        public COLORREF(UnityEngine.Color color)
        {
            this.r = (byte)Mathf.Lerp(byte.MinValue, byte.MaxValue, color.r);
            this.g = (byte)Mathf.Lerp(byte.MinValue, byte.MaxValue, color.g);
            this.b = (byte)Mathf.Lerp(byte.MinValue, byte.MaxValue, color.b);
            this.padding = 0;
        }

        public static unsafe implicit operator uint(COLORREF cref)
        {
            return *(uint*)&cref;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

    public static Point ScreenToClient(this Point point, IntPtr hWnd)
    {
        ScreenToClient(hWnd, ref point);
        return point;
    }

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size != 8)
            return new IntPtr(SetWindowLong32(hWnd, nIndex, (int)dwNewLong));
        else
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("Dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    public static bool FollowWindow(string title, string className)
    {
#if TEST_A
        IntPtr fore = GetForegroundWindow();

        if (!CompareTitleAndClass(fore, title, className))
            return false;

        RECT r;
        GetWindowRect(fore, out r);
        Point local = r.TopLeft.ScreenToClient(Active);
        SetWindowPos(Active, HWND_TOPMOST, local.X, local.Y, r.Width, r.Height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);

        return true;
#else
        return CompareTitleAndClass(GetForegroundWindow(), title, className);
#endif
    }

    public static void MakeOverlay(UnityEngine.Color? key = null)
    {
#if TEST_A
        SetWindowLongPtr(Active, -16, new IntPtr(0x80000000 | 0x00040000));
#endif
        // Transparent, click-through.
        SetWindowLongPtr(Active, GWL_EX_STYLE, new IntPtr(WS_EX_LAYERED | WS_EX_TRANSPARENT));

        if (key.HasValue)
        {
            SetLayeredWindowAttributes(Active, new COLORREF(key.Value), byte.MaxValue, LWA_COLORKEY);
        }
        else
        {
            MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(Active, ref margins);
        }

        // Topmost.
        SetWindowPos(Active, HWND_TOPMOST, 0, 0, Screen.width, Screen.height, SWP_FRAMECHANGED | SWP_SHOWWINDOW);
    }

    public static bool Compare(IntPtr hWnd, Func<IntPtr, StringBuilder, int, int> getter, string to, bool ignoreCase = false)
    {
        StringBuilder result = new StringBuilder(to.Length + 1);
        getter(hWnd, result, result.Capacity);
        return string.Compare(result.ToString(), to, ignoreCase, CultureInfo.InvariantCulture) == 0;
    }

    public static bool CompareTitleAndClass(IntPtr hWnd, string title, string className)
    {
        return Compare(hWnd, GetWindowText, title) && Compare(hWnd, GetClassName, className, true);
    }
}