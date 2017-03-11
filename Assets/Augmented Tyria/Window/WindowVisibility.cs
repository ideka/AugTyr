using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class WindowVisibility : MonoBehaviour
{
#if !UNITY_EDITOR
    public const string GameWindowTitle = "Guild Wars 2";
    public const string MainSceneName = "Main";
    public const string OffSceneName = "Off";

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    private int defaultCullingMask;

    public Camera Camera { get; private set; }

    private void Awake()
    {
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
    }

    private void Update()
    {
        StringBuilder name = new StringBuilder();
        GetWindowText(GetForegroundWindow(), name, GameWindowTitle.Length + 2);  // ???: Why do I have to add 2 here...?

        if (name.ToString() == GameWindowTitle)
            this.Camera.cullingMask = this.defaultCullingMask;
        else
            this.Camera.cullingMask = 0;
    }
#endif
}