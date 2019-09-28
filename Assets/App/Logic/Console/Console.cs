using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Console : MonoBehaviour, IActionable
{
    public ConsoleMessage MessagePrefab;
    public UserConfigHolder UserConfigHolder;

    public const float DefaultFadeTime = 4;

    public bool Hidden { get; private set; }

    public MonoBehaviour Holder { get => this; }
    public UserConfig UserConfig { get => this.UserConfigHolder.UserConfig; }
    Console IActionable.Console { get => this; }

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

    public CanvasGroup CanvasGroup { get; private set; }

    private void Awake()
    {
        this.CanvasGroup = this.GetComponent<CanvasGroup>();

        this.SetUp();
    }

    private void LateUpdate()
    {
        this.CanvasGroup.alpha = this.Hidden ? 0 : 1;
    }

    public void Message(ConsoleMessageType type, string message, float fadeOutTime = -1)
    {
        if (this.UserConfig.ConsoleFilter > (int)type)
            return;

        ConsoleMessage[] sent = this.transform.GetComponentsInChildren<ConsoleMessage>();
        bool permanent = fadeOutTime < 0;

        if (!sent.Any() || !sent.Last().TryAddOne(type, message, permanent))
        {
            ConsoleMessage msg = this.transform.Instantiate(this.MessagePrefab);
            msg.SetUp(type, message, this.UserConfig.ConsoleFontSize,
                permanent ? -1 : fadeOutTime + Mathf.Max(0, sent.Select(cm => cm.FadeOutTimeLeft).DefaultIfEmpty(0).Max()));
        }
    }

    public void Info(string message, params object[] args) => this.Info(false, message, args);
    public void InfoFade(string message, params object[] args) => this.Info(true, message, args);
    public void Info(bool fadeOut, string message, params object[] args)
        => this.Message(ConsoleMessageType.Info, string.Format(message, args), fadeOut ? DefaultFadeTime : -1);

    public void Warning(string message, params object[] args) => this.Warning(false, message, args);
    public void WarningFade(string message, params object[] args) => this.Warning(true, message, args);
    public void Warning(bool fadeOut, string message, params object[] args)
        => this.Message(ConsoleMessageType.Warning, string.Format(message, args), fadeOut ? DefaultFadeTime : -1);

    public void Error(string message, params object[] args) => this.Error(false, message, args);
    public void ErrorFade(string message, params object[] args) => this.Error(true, message, args);
    public void Error(bool fadeOut, string message, params object[] args)
        => this.Message(ConsoleMessageType.Error, string.Format(message, args), fadeOut ? DefaultFadeTime : -1);

    public void ToggleHide()
    { 
        this.Hidden = !this.Hidden;
    }

    public void Clear()
    {
        if (!this.Hidden)
            foreach (Transform t in this.transform)
                Destroy(t.gameObject);
    }
}