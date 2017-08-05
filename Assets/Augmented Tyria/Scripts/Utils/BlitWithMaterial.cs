using System.IO;
using UnityEngine;

public class BlitWithMaterial : MonoBehaviour
{
    [SerializeField]
    private Material material;

    private int c = 0;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.material != null)
            Graphics.Blit(source, destination, this.material);
        else
            Graphics.Blit(source, destination);

        c++;
        if (c == 300)
        {
            Debug.Log("ASDASFDSDFASDFDF");
            var rt = destination;

            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;

            // Set the supplied RenderTexture as the active one
            RenderTexture.active = rt;

            // Create a new Texture2D and read the RenderTexture image into it
            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

            // Restorie previously active render texture
            RenderTexture.active = currentActiveRT;

            File.WriteAllBytes(string.Format("C:/Users/x/Desktop/{0}.png", this.gameObject.name), tex.EncodeToPNG());
        }
    }
}