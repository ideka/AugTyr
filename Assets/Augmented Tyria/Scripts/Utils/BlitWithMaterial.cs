using UnityEngine;

public class BlitWithMaterial : MonoBehaviour
{
    [SerializeField]
    private Material Material;

    private void OnRenderImage(RenderTexture from, RenderTexture to)
    {
        Graphics.Blit(from, to, this.Material);
    }
}