using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ClearCamera : MonoBehaviour
{
    public UserConfigHolder UserConfigHolder;
    public Material Material;

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }

    private void Awake()
    {
#if !UNITY_EDITOR
        if (!this.UserConfig.ByColorTransparency)
        {
            WinAPI.MakeOverlay();
            return;
        }

        WinAPI.MakeOverlay(this.GetComponent<Camera>().backgroundColor);
#endif
        Destroy(this);
    }

#if !UNITY_EDITOR
    private void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, this.Material);
    }
#endif
}