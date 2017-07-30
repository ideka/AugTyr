using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Console : MonoBehaviour, IActionable
{
    public ConsoleMessage MessagePrefab;
    public UserConfigHolder UserConfigHolder;

    public const float DefaultFadeTime = 4;

    public bool Hidden { get; private set; }

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }
    Console IActionable.Console { get { return this; } }

    public string InputGroupName { get { return "Console"; } }
    public Dictionary<string, Action> Actions
    {
        get
        {
            return new Dictionary<string, Action>
            {
                { "ToggleHide", this.ToggleHide },
                { "Clear", this.Clear }
            };
        }
    }

    private IKeyboardMouseEvents globalHook;

    public CanvasGroup CanvasGroup { get; private set; }

    private void Awake()
    {
        this.CanvasGroup = this.GetComponent<CanvasGroup>();

        this.globalHook = Hook.GlobalEvents();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;
    }

    private void OnDestroy()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.Dispose();
    }

    private void LateUpdate()
    {
        this.CanvasGroup.alpha = this.Hidden || this.transform.childCount == 0 ? 0 : 1;
    }

    public void Print(ConsoleMessageType type, string message, float fadeOutTime = -1)
    {
        if (this.UserConfig.ConsoleFilter > (int)type)
            return;
        ConsoleMessage msg = Instantiate(this.MessagePrefab.gameObject, this.transform).GetComponent<ConsoleMessage>();
        msg.SetUp(type, message, fadeOutTime);
    }

    public void PrintInfo(bool fadeOut, string message, params object[] arg)
    {
        this.Print(ConsoleMessageType.Info, string.Format(message, arg), fadeOut ? DefaultFadeTime : -1);
    }

    public void PrintWarning(bool fadeOut, string message, params object[] arg)
    {
        this.Print(ConsoleMessageType.Warning, string.Format(message, arg), fadeOut ? DefaultFadeTime : -1);
    }

    public void PrintError(bool fadeOut, string message, params object[] arg)
    {
        this.Print(ConsoleMessageType.Error, string.Format(message, arg), fadeOut ? DefaultFadeTime : -1);
    }

    public void ToggleHide()
    {
        this.Hidden = !this.Hidden;
    }

    public void Clear()
    {
        if (this.CanvasGroup.alpha == 1)
            foreach (Transform t in this.transform)
                Destroy(t.gameObject);
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        this.Act(e.KeyCode, e.Control);
    }
}