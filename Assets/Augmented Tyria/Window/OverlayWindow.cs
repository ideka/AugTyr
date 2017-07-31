#if !UNITY_EDITOR
using System;
#endif
using UnityEngine;

public class OverlayWindow : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_EDITOR
        IntPtr hWnd = WinAPI.GetActiveWindow();

        // Transparent.
        IntPtr style = WinAPI.GetWindowLongPtr(hWnd, WinAPI.GWL_EX_STYLE);
        WinAPI.SetWindowLongPtr(hWnd, WinAPI.GWL_EX_STYLE, new IntPtr((int)style | WinAPI.WS_EX_LAYERED | WinAPI.WS_EX_TRANSPARENT));
        WinAPI.SetLayeredWindowAttributes(hWnd, 0, 255, WinAPI.LWA_ALPHA);

        WinAPI.MARGINS margins = new WinAPI.MARGINS() { cxLeftWidth = -1 };
        WinAPI.DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // Topmost.
        WinAPI.SetWindowPos(hWnd, WinAPI.HWND_TOPMOST, 0, 0, Screen.width, Screen.height, WinAPI.SWP_FRAMECHANGED | WinAPI.SWP_SHOWWINDOW);
#endif

        Destroy(this);
    }
}