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
    public Material TeleportMaterial;
    public Material HeartMaterial;
    public Material HeartWallMaterial;

    public const float PulsateRatio = 3 / 5f;
    public const float PulsateSpeed = 3f;

    public float NormalizedSize { get; private set; }

    public float Size
    {
        set
        {
            this.SetScale(value);
            this.size = value;
        }
    }

    public bool Detached
    {
        set
        {
            this.MeshFilter.mesh = value ? this.DetachedMesh : this.AttachedMesh;
        }
    }

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

                case NodeType.Teleport:
                    this.MeshRenderer.material = this.TeleportMaterial;
                    if (!string.IsNullOrEmpty(value.WaypointCode))
                        this.Text.text += "\n\nWP: " + value.WaypointCode;
                    else if (string.IsNullOrEmpty(this.Text.text))
                        this.Text.text = "Teleport.";
                    break;

                case NodeType.Heart:
                    this.MeshRenderer.material = this.HeartMaterial;
                    break;

                case NodeType.HeartWall:
                    this.MeshRenderer.material = this.HeartWallMaterial;
                    this.Text.text += "\n\nHeartWall: " + (value.HeartWallValue ?? "Any") + "%";
                    break;
            }

            this.Text.text = this.Text.text.Trim();

            this.Canvas.gameObject.SetActive(!string.IsNullOrEmpty(this.Text.text));
        }
    }

    private float size = .3f;

    private void Start()
    {
        this.SetScale(this.size);
    }

    public void SetUp(float size, bool detached, Node node)
    {
        this.Size = size;
        this.Detached = detached;
        this.Node = node;
    }

    public void Select(bool select)
    {
        this.StopAllCoroutines();

        if (select)
            this.StartCoroutine(this.Pulsating());
        else
            this.SetScale(this.size);
    }

    private void SetScale(float to)
    {
        this.MeshRenderer.transform.localScale = Vector3.one * to;
    }

    private IEnumerator Pulsating()
    {
        this.NormalizedSize = .5f;
        while (true)
        {
            yield return this.PulsatingTowards(1);
            yield return this.PulsatingTowards(0);
        }
    }

    private IEnumerator PulsatingTowards(float goal)
    {
        while (this.NormalizedSize != goal)
        {
            this.NormalizedSize = Mathf.MoveTowards(this.NormalizedSize, goal, PulsateSpeed * Time.deltaTime);
            this.SetScale(Mathf.Lerp(
                this.size - this.size * PulsateRatio,
                this.size + this.size * PulsateRatio,
                this.NormalizedSize));
            yield return null;
        }
    }
}