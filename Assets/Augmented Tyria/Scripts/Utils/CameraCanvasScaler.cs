using UnityEngine;
using UnityEngine.UI;

public class CameraCanvasScaler : CanvasScaler
{
    // The log base doesn't have any influence on the results whatsoever, as long as the same base is used everywhere.
    public const float kLogBase = 2;

    private Canvas newM_Canvas;

    protected override void OnEnable()
    {
        newM_Canvas = GetComponent<Canvas>();
        base.OnEnable();
    }

    protected override void HandleScaleWithScreenSize()
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        if (newM_Canvas.renderMode == RenderMode.ScreenSpaceCamera && newM_Canvas.worldCamera != null)
        {
            screenSize.x *= newM_Canvas.worldCamera.rect.width;
            screenSize.y *= newM_Canvas.worldCamera.rect.height;
        }

        float scaleFactor = 0;
        switch (m_ScreenMatchMode)
        {
            case ScreenMatchMode.MatchWidthOrHeight:
            {
                // We take the log of the relative width and height before taking the average.
                // Then we transform it back in the original space.
                // the reason to transform in and out of logarithmic space is to have better behavior.
                // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
                // In normal space the average would be (0.5 + 2) / 2 = 1.25
                // In logarithmic space the average is (-1 + 1) / 2 = 0
                float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
                float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);
                float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
                scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);
                break;
            }
            case ScreenMatchMode.Expand:
            {
                scaleFactor = Mathf.Min(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                break;
            }
            case ScreenMatchMode.Shrink:
            {
                scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);
                break;
            }
        }

        SetScaleFactor(scaleFactor);
        SetReferencePixelsPerUnit(m_ReferencePixelsPerUnit);
    }
}