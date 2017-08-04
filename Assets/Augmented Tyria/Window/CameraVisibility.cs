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

    public static bool Focused { get; private set; }

#if !UNITY_EDITOR
    public Camera Camera { get; private set; }
#endif

    private void Awake()
    {
#if !UNITY_EDITOR
        this.Camera = this.GetComponent<Camera>();
#else
        Focused = true;
        Destroy(this);
#endif
    }

#if !UNITY_EDITOR
    private void LateUpdate()
    {
        IntPtr hWnd = WinAPI.GetForegroundWindow();
        Focused = WinAPI.CompareTitleAndClass(hWnd, GameWindowTitle, GameWindowClass);
        if (Focused)
        {
            WinAPI.RECT cr;
            WinAPI.GetClientRect(hWnd, out cr);
            cr = cr.ClientToScreen(hWnd);

            this.Camera.pixelRect = new Rect(cr.left, cr.bottom, cr.Width, cr.Height);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);
        }
    }
#endif
}