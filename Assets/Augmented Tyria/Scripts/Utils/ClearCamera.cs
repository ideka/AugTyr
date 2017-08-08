using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ClearCamera : MonoBehaviour
{
    public UserConfigHolder UserConfigHolder;
    public Material FullTransparency;
    public Material DiscardAll;

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }

    private void Awake()
    {
#if !UNITY_EDITOR
        if (this.UserConfig.TransparencyMethod == TransparencyMethod.ByColor)
        {
            WinAPI.MakeOverlay(this.GetComponent<Camera>().backgroundColor);
            Destroy(this);
        }

        WinAPI.MakeOverlay();
#else
        Destroy(this);
#endif
    }

#if !UNITY_EDITOR
    private void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        switch (this.UserConfig.TransparencyMethod)
        {
            case TransparencyMethod.ConstantBlit:
                Graphics.Blit(from, to, this.FullTransparency);
                break;

            case TransparencyMethod.DiscardAllBlit:
                Graphics.Blit(from, to, this.DiscardAll);
                break;

            case TransparencyMethod.DefaultBlit:
                Graphics.Blit(from, to);
                break;

            case TransparencyMethod.NeglectBlit:
                // Do nothing.
                break;
        }
    }
#endif
}