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

    public static bool Focused { get; private set; }
#else
    public const bool Focused = true;
#endif

#if !UNITY_EDITOR
    private int defaultCullingMask;

    public Camera Camera { get; private set; }
#endif

    private void Awake()
    {
#if !UNITY_EDITOR
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
        this.Camera.cullingMask = 0;
#else
        Destroy(this);
#endif
    }

#if !UNITY_EDITOR
    private void LateUpdate()
    {
        IntPtr fore = WinAPI.GetForegroundWindow();
        Focused = WinAPI.CompareTitleAndClass(fore, GameWindowTitle, GameWindowClass);

        if (Focused)
        {
            WinAPI.RECT cr;
            WinAPI.GetClientRect(fore, out cr);
            cr = cr.ClientToScreen(fore);

            this.Camera.pixelRect = new Rect(cr.left, cr.bottom, cr.Width, cr.Height);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);

            this.Camera.cullingMask = this.defaultCullingMask;
        }
        else
        {
            this.Camera.cullingMask = 0;
        }
    }
#endif
}