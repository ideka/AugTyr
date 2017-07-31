#if !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class WinAPI
{
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    public const int GWL_EX_STYLE = -20;
    public const uint WS_EX_LAYERED = 0x80000;
    public const uint WS_EX_TRANSPARENT = 0x20;

    public const uint LWA_ALPHA = 2;

    public const uint SWP_FRAMECHANGED = 0x20;
    public const uint SWP_SHOWWINDOW = 0x40;

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int LeftWidth;
        public int RightWidth;
        public int TopHeight;
        public int BottomHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int X { get { return this.Left; } }
        public int Y { get { return this.Top; } }
        public int Width { get { return this.Right - this.Left; } }
        public int Height { get { return this.Bottom - this.Top; } }
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern int SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("Dwmapi.dll")]
    public static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
}
#endif