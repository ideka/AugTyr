using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ConsoleMessage : MonoBehaviour
{
    public AnimationCurve FadeOutCurve;

    public static readonly Color InfoColor = new Color(.2f, .2f, .2f);
    public static readonly Color WarningColor = new Color(.6f, .6f, 0);
    public static readonly Color ErrorColor = new Color(.5f, 0, 0);

    public float FadeOutTimeLeft { get; private set; }

    private ConsoleMessageType type;
    private string originalText;
    private int timesSent;
    private float fadeOutTime;

    public Text Text { get; private set; }

    private void Awake()
    {
        this.Text = this.GetComponent<Text>();
    }

    private void Update()
    {
        if (this.fadeOutTime != -1)
        {
            this.FadeOutTimeLeft -= Time.deltaTime;

            if (this.FadeOutTimeLeft <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Color c = this.Text.color;
                c.a = this.FadeOutCurve.Evaluate(Mathf.InverseLerp(this.fadeOutTime, 0, this.FadeOutTimeLeft));
                this.Text.color = c;
            }
        }
    }

    public void SetUp(ConsoleMessageType type, string text, float fadeOutTime = -1)
    {
        this.type = type;
        this.originalText = this.Text.text = text;
        this.timesSent = 1;
        this.fadeOutTime = this.FadeOutTimeLeft = fadeOutTime;
        this.Text.color = ColorFor(this.type);
    }

    public bool TryAddOne(ConsoleMessageType type, string text, bool makePermanent)
    {
        if (type != this.type || text != this.originalText)
            return false;

        this.timesSent++;
        this.fadeOutTime = this.FadeOutTimeLeft = makePermanent ? -1 : this.fadeOutTime;
        this.Text.text = string.Format("{0} (x{1})", this.originalText, this.timesSent);
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