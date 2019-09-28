using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ConsoleMessage : MonoBehaviour
{
    public AnimationCurve FadeOutCurve;

    public static readonly Color InfoColor = new Color(.35f, .35f, .35f);
    public static readonly Color WarningColor = new Color(.6f, .6f, 0);
    public static readonly Color ErrorColor = new Color(.5f, 0, 0);

    public float FadeOutTimeLeft { get; private set; }

    private ConsoleMessageType _type;
    private string _originalText;
    private int _timesSent;
    private float _fadeOutTime;

    public Text Text { get; private set; }

    private void Awake()
    {
        this.Text = this.GetComponent<Text>();
    }

    private void Update()
    {
        if (this._fadeOutTime != -1)
        {
            this.FadeOutTimeLeft -= Time.deltaTime;

            if (this.FadeOutTimeLeft <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Color c = this.Text.color;
                c.a = this.FadeOutCurve.Evaluate(Mathf.InverseLerp(this._fadeOutTime, 0, this.FadeOutTimeLeft));
                this.Text.color = c;
            }
        }
    }

    public void SetUp(ConsoleMessageType type, string text, int fontSize, float fadeOutTime = -1)
    {
        this._type = type;
        this._originalText = this.Text.text = text;
        this._timesSent = 1;
        this.Text.fontSize = fontSize;
        this._fadeOutTime = this.FadeOutTimeLeft = fadeOutTime;
        this.Text.color = ColorFor(this._type);
    }

    public bool TryAddOne(ConsoleMessageType type, string text, bool makePermanent)
    {
        if (type != this._type || text != this._originalText)
            return false;

        this._timesSent++;
        this._fadeOutTime = this.FadeOutTimeLeft = makePermanent ? -1 : this._fadeOutTime;
        this.Text.text = string.Format("{0} (x{1})", this._originalText, this._timesSent);
        return true;
    }

    private static Color ColorFor(ConsoleMessageType type)
    {
        switch (type)
        {
            case ConsoleMessageType.Info:
                return InfoColor;

            case ConsoleMessageType.Warning:
                return WarningColor;

            case ConsoleMessageType.Error:
                return ErrorColor;
        }

        return new Color();
    }
}

public enum ConsoleMessageType
{
    Info,
    Warning,
    Error
}