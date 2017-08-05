using UnityEngine;

public class ClearCamera : MonoBehaviour
{
    public UserConfigHolder UserConfigHolder;

    public UserConfig UserConfig { get { return this.UserConfigHolder.UserConfig; } }

    private void Awake()
    {
#if !UNITY_EDITOR
        if (!this.UserConfig.ByColorTransparency)
            WinAPI.MakeOverlay();
        else
            WinAPI.MakeOverlay(this.GetComponent<Camera>().backgroundColor);
#endif
        Destroy(this);
    }
}