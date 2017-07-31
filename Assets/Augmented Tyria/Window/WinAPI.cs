#if !UNITY_EDITOR
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

public static class WinAPI
{
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    public const int GWL_EX_STYLE = -20;
    public const int WS_EX_LAYERED = 0x80000;
    public const int WS_EX_TRANSPARENT = 0x20;

    public const uint LWA_ALPHA = 2;

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
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", EntryPoint="GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint="GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
         if (IntPtr.Size != 8)
             return new IntPtr(GetWindowLong32(hWnd, nIndex));
         else
             return GetWindowLongPtr64(hWnd, nIndex);
    }

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
#endif