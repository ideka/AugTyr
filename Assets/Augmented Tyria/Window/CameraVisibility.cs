#if !UNITY_EDITOR
using System;
using System.Text;
#endif
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVisibility : MonoBehaviour
{
#if !UNITY_EDITOR
    public const string GameWindowTitle = "Guild Wars 2";

    private int defaultCullingMask;

    public Camera Camera { get; private set; }
#endif

    private void Awake()
    {
#if !UNITY_EDITOR
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
#else
        Destroy(this);
#endif
    }

#if !UNITY_EDITOR
    private void LateUpdate()
    {
        // TODO: Use a more strict method to tell if the focused window is Guild Wars 2.
        IntPtr hWnd = WinAPI.GetForegroundWindow();

        StringBuilder name = new StringBuilder();
        WinAPI.GetWindowText(hWnd, name, GameWindowTitle.Length + 2);  // ???: Why do I have to add 2 here...?

        if (name.ToString() == GameWindowTitle)
        {
            this.Camera.cullingMask = this.defaultCullingMask;

            WinAPI.RECT r;
            WinAPI.GetWindowRect(hWnd, out r);

            this.Camera.pixelRect = new Rect(r.Left, r.Bottom, r.Width, r.Height);
            this.Camera.rect = new Rect(this.Camera.rect.x, 1 - this.Camera.rect.y, this.Camera.rect.width, this.Camera.rect.height);
        }
        else
        {
            this.Camera.cullingMask = 0;
        }
    }
#endif
}