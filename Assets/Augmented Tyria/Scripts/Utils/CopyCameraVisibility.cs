using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CopyCameraVisibility : MonoBehaviour
{
    private int defaultCullingMask;

    public Camera Camera { get; private set; }

    private void Awake()
    {
        this.Camera = this.GetComponent<Camera>();

        this.defaultCullingMask = this.Camera.cullingMask;
    }

    private void LateUpdate()
    {
        this.Camera.cullingMask = Camera.main.cullingMask == 0 ? 0 : this.defaultCullingMask;
        this.Camera.rect = Camera.main.rect;
    }
}