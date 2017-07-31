#if !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;
#endif
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVisibility : MonoBehaviour
{
#if !UNITY_EDITOR
    public const string GameWindowTitle = "Guild Wars 2";

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private int defaultCullingMask;

    public Camera Camera { get; private set; }

    private void Awake()
    {
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
    }

    private void LateUpdate()
    {
        IntPtr hWnd = GetForegroundWindow();

        StringBuilder name = new StringBuilder();
        GetWindowText(hWnd, name, GameWindowTitle.Length + 2);  // ???: Why do I have to add 2 here...?

        if (name.ToString() == GameWindowTitle)
        {
            this.Camera.cullingMask = this.defaultCullingMask;

            RECT r = new RECT();
            GetWindowRect(hWnd, ref r);
            this.Camera.pixelRect = new Rect(r.Left, r.Bottom, r.Right - r.Left, r.Bottom - r.Top);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);
        }
        else
        {
            this.Camera.cullingMask = 0;
        }
    }
#endif
}