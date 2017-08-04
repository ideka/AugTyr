#if !UNITY_EDITOR
using System;
#endif
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVisibility : MonoBehaviour
{
#if !UNITY_EDITOR
    public const string GameWindowTitle = "Guild Wars 2";
    public const string GameWindowClass = "ArenaNet_Dx_Window_Class";
#endif

    private static bool focused;
    public static bool Focused { get { return focused; } }

#if !UNITY_EDITOR
    private IntPtr gameWindow = IntPtr.Zero;
#endif
    private int defaultCullingMask;

#if !UNITY_EDITOR
    public Camera Camera { get; private set; }
#endif

    private void Awake()
    {
#if !UNITY_EDITOR
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
#else
        focused = true;
        Destroy(this);
#endif
    }

#if !UNITY_EDITOR
    private void LateUpdate()
    {
        bool shouldLayOver = false;

        if (this.gameWindow == IntPtr.Zero || !WinAPI.IsWindow(this.gameWindow) || !WinAPI.CompareTitleAndClass(this.gameWindow, GameWindowTitle, GameWindowClass))
        {
            this.gameWindow = WinAPI.FindWindow(GameWindowClass, GameWindowTitle);
            shouldLayOver |= this.gameWindow != IntPtr.Zero;
        }

        if (this.gameWindow != IntPtr.Zero)
        {
            shouldLayOver |= (this.gameWindow == WinAPI.GetForegroundWindow()).SetIfDiff(ref focused);

            WinAPI.RECT cr;
            WinAPI.GetClientRect(this.gameWindow, out cr);
            cr = cr.ClientToScreen(this.gameWindow);

            this.Camera.pixelRect = new Rect(cr.left, cr.bottom, cr.Width, cr.Height);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);

            this.Camera.cullingMask = this.defaultCullingMask;

            if (shouldLayOver)
            {
                WinAPI.SetWindowPos(WinAPI.Active, this.gameWindow, 0, 0, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOMOVE);
                WinAPI.SetWindowPos(this.gameWindow, WinAPI.Active, 0, 0, 0, 0, WinAPI.SWP_NOSIZE | WinAPI.SWP_NOMOVE);
            }
        }
        else
        {
            focused = false;
            this.Camera.cullingMask = 0;
        }
    }
#endif
}