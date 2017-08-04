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

    public const int GWL_HWNDPARENT = -8;

    public const int GWL_STYLE = -16;
    public const uint WS_CLIPSIBLINGS = 0x4000000;
    public const uint WS_VISIBLE = 0x10000000;
    public const uint WS_CHILD = 0x40000000;
    public const uint WS_POPUP = 0x80000000;

    public const int GWL_EX_STYLE = -20;
    public const uint WS_EX_LAYERED = 0x80000;
    public const uint WS_EX_TRANSPARENT = 0x20;

    public const uint LWA_COLORKEY = 1;
    public const uint LWA_ALPHA = 2;

    public const uint SWP_NOSIZE = 0x1;
    public const uint SWP_NOMOVE = 0x2;
    public const uint SWP_FRAMECHANGED = 0x20;
    public const uint SWP_SHOWWINDOW = 0x40;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width { get { return this.right - this.left; } }
        public int Height { get { return this.bottom - this.top; } }

        public RECT ClientToScreen(IntPtr hWnd)
        {
            return this.Move(p => p.ClientToScreen(hWnd));
        }

        public RECT ScreenToClient(IntPtr hWnd)
        {
            return this.Move(p => p.ScreenToClient(hWnd));
        }

        private RECT Move(Func<Point, Point> mover)
        {
            Point lt = mover(new Point(this.left, this.top));
            return new RECT()
            {
                left = lt.X,
                top = lt.Y,
                right = this.right + lt.X - this.left,
                bottom = this.bottom + lt.Y - this.top
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
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

    public static Point ScreenToClient(this Point point, IntPtr hWnd)
    {
        ScreenToClient(hWnd, ref point);
        return point;
    }

    [DllImport("user32.dll")]
    public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    public static Point ClientToScreen(this Point point, IntPtr hWnd)
    {
        ClientToScreen(hWnd, ref point);
        return point;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, UIntPtr dwNewLong);

    public static int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong)
    {
        if (IntPtr.Size != 8)
            return SetWindowLong32(hWnd, nIndex, dwNewLong);
        else
            return (int)SetWindowLongPtr64(hWnd, nIndex, new UIntPtr(dwNewLong));
    }

    [DllImport("user32.dll", EntryPoint="GetWindowLong")]
    private static extern uint GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint="GetWindowLongPtr")]
    private static extern UIntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public static uint GetWindowLong(IntPtr hWnd, int nIndex)
    {
         if (IntPtr.Size != 8)
             return GetWindowLong32(hWnd, nIndex);
         else
             return (uint)GetWindowLongPtr64(hWnd, nIndex);
    }

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("Dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    public static void MakeOverlay(UnityEngine.Color? key = null)
    {
        // Layered, click-through.
        SetWindowLong(Active, GWL_EX_STYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        // Transparency.
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
        SetWindowPos(Active, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED | SWP_SHOWWINDOW);
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