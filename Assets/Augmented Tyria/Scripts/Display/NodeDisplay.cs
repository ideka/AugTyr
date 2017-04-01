using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NodeDisplay : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public MeshRenderer MeshRenderer;
    public Canvas Canvas;
    public Text Text;

    [Header("Meshes")]
    public Mesh AttachedMesh;
    public Mesh DetachedMesh;

    [Header("Materials")]
    public Material ReachMaterial;
    public Material WaypointMaterial;
    public Material HeartMaterial;
    public Material HeartWallMaterial;

    public const float PulsateMin = .1f;
    public const float PulsateMax = .5f;
    public const float PulsateSpeed = 2;

    public Node Node
    {
        set
        {
            this.transform.position = value.Position;
            this.Text.text = value.Comment;

            switch (value.Type)
            {
                case NodeType.Reach:
                    this.MeshRenderer.material = this.ReachMaterial;
                    break;

                case NodeType.Waypoint:
                    this.MeshRenderer.material = this.WaypointMaterial;
                    this.Text.text += "\n\nWP: " + value.WaypointCode;
                    break;

                case NodeType.Heart:
                    this.MeshRenderer.material = this.HeartMaterial;
                    break;

                case NodeType.HeartWall:
                    this.MeshRenderer.material = this.HeartWallMaterial;
                    this.Text.text += "\n\nHeartWall: " + value.HeartWallValue + "%";
                    break;
            }

            this.Canvas.gameObject.SetActive(this.Text.text != null && this.Text.text != "");
        }
    }

    public void SetMesh(bool detached)
    {
        this.MeshFilter.mesh = detached ? this.DetachedMesh : this.AttachedMesh;
    }

    public void Select(bool select)
    {
        this.StopAllCoroutines();

        if (select)
            this.StartCoroutine(this.Pulsating());
        else
            this.MeshRenderer.transform.localScale = Vector3.Lerp(Vector3.one * PulsateMax, Vector3.one * PulsateMin, .5f);
    }

    private IEnumerator Pulsating()
    {
        while (true)
        {
            while (this.MeshRenderer.transform.localScale.x < PulsateMax)
            {
                this.MeshRenderer.transform.localScale = Vector3.MoveTowards(
                    this.MeshRenderer.transform.localScale,
                    Vector3.one * PulsateMax,
                    Time.deltaTime * PulsateSpeed);
                yield return null;
            }

            while (this.MeshRenderer.transform.localScale.x > PulsateMin)
            {
                this.MeshRenderer.transform.localScale = Vector3.MoveTowards(
                    this.MeshRenderer.transform.localScale,
                    Vector3.one * PulsateMin,
                    Time.deltaTime * PulsateSpeed);
                yield return null;
            }
        }
    }
}