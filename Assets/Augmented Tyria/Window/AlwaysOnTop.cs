#if !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
#endif
using UnityEngine;

public class AlwaysOnTop : MonoBehaviour
{
#if !UNITY_EDITOR
    public const string ToolWindowTitle = "Augmented Tyria";

    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    public static readonly IntPtr HWND_NOT_TOPMOST = new IntPtr(-2);
    public const UInt32 SWP_SHOWWINDOW = 0x0040;

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int X
        {
            get { return this.Left; }

            set
            {
                this.Right -= this.Left - value;
                this.Left = value;
            }
        }

        public int Y
        {
            get { return this.Top; }

            set
            {
                this.Bottom -= this.Top - value;
                this.Top = value;
            }
        }

        public int Width
        {
            get { return this.Right - this.Left; }
            set { this.Right = value + this.Left; }
        }

        public int Height
        {
            get { return this.Bottom - this.Top; }
            set { this.Bottom = value + this.Top; }
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private void Start()
    {
        IntPtr hWnd = FindWindow(null, ToolWindowTitle);

        RECT rect;
        GetWindowRect(new HandleRef(this, hWnd), out rect);
        SetWindowPos(hWnd, HWND_TOPMOST, rect.X, rect.Y, rect.Width, rect.Height, SWP_SHOWWINDOW);

        Destroy(this);
    }
#endif
}
