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
        this.Text.text = text;
        this.fadeOutTime = this.FadeOutTimeLeft = fadeOutTime;

        switch (type)
        {
            case ConsoleMessageType.Info:
                this.Text.color = InfoColor;
                break;

            case ConsoleMessageType.Warning:
                this.Text.color = WarningColor;
                break;

            case ConsoleMessageType.Error:
                this.Text.color = ErrorColor;
                break;
        }
    }
}

public enum ConsoleMessageType
{
    Info,
    Warning,
    Error
}