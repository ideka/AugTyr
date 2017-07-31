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
        int style = WinAPI.GetWindowLong(hWnd, WinAPI.GWL_EX_STYLE);
        WinAPI.SetWindowLong(hWnd, WinAPI.GWL_EX_STYLE, (uint)style | WinAPI.WS_EX_LAYERED | WinAPI.WS_EX_TRANSPARENT);
        WinAPI.SetLayeredWindowAttributes(hWnd, 0, 255, WinAPI.LWA_ALPHA);

        WinAPI.MARGINS margins = new WinAPI.MARGINS() { LeftWidth = -1 };
        WinAPI.DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // Topmost.
        WinAPI.SetWindowPos(hWnd, WinAPI.HWND_TOPMOST, 0, 0, Screen.width, Screen.height, 32 | 64);  // SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
#endif

        Destroy(this);
    }
}