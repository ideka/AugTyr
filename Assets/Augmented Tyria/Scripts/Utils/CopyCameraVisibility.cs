using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CopyCameraVisibility : MonoBehaviour
{
    public Camera Camera { get; private set; }

    private void Awake()
    {
        this.Camera = this.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        this.Camera.rect = Camera.main.rect;
    }
}