using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasVisibility : MonoBehaviour
{
    public CanvasGroup CanvasGroup { get; private set; }

    private void Awake()
    {
        this.CanvasGroup = this.GetComponent<CanvasGroup>();
    }

    private void LateUpdate()
    {
        this.CanvasGroup.alpha = Camera.main.cullingMask == 0 ? 0 : 1;
    }
}