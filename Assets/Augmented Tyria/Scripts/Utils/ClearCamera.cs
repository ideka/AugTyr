using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ClearCamera : MonoBehaviour
{
    public UserConfigHolder UserConfigHolder;
    public Material Material;

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }

#if !UNITY_EDITOR
    private void Awake()
    {
        if (!this.UserConfig.ByColorTransparency)
        {
            WinAPI.MakeOverlay();
        }
        else
        {
            WinAPI.MakeOverlay(this.GetComponent<Camera>().backgroundColor);
            Destroy(this);
        }
    }

    private void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, this.Material);
    }
#endif
}