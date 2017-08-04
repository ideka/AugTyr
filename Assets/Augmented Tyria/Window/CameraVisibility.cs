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
        this.Camera.cullingMask = WinAPI.FollowWindow(GameWindowTitle, GameWindowClass) ? this.defaultCullingMask : 0;

        /*IntPtr hWnd = WinAPI.GetForegroundWindow();
        if (WinAPI.CompareTitleAndClass(hWnd, GameWindowTitle, GameWindowClass))
        {
            this.Camera.cullingMask = this.defaultCullingMask;

            WinAPI.RECT r;
            WinAPI.GetWindowRect(hWnd, out r);

            this.Camera.pixelRect = new Rect(r.left, r.bottom, r.Width, r.Height);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);
        }
        else
        {
            this.Camera.cullingMask = 0;
        }*/
    }
#endif
}