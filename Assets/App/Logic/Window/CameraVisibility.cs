using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVisibility : MonoBehaviour
{
    public const string GameWindowTitle = "Guild Wars 2";
    public const string GameWindowClass = "ArenaNet_Dx_Window_Class";

    public static bool Focused { get; private set; }

    private int defaultCullingMask;

    public Camera Camera { get; private set; }

    private void Awake()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            Focused = true;
            Destroy(this);
            return;
        }
#endif

        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
        this.Camera.cullingMask = 0;
    }

    private void LateUpdate()
    {
        IntPtr fore = WinAPI.GetForegroundWindow();
        Focused = WinAPI.CompareTitleAndClass(fore, GameWindowTitle, GameWindowClass);

        if (Focused)
        {
            WinAPI.GetClientRect(fore, out WinAPI.RECT cr);
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
}