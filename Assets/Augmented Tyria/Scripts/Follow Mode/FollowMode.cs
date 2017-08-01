using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public class FollowMode : MonoBehaviour, INodeRoute, IActionable
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public Material FollowMaterial;
    public Material HeartMaterial;

    public NodeDisplay NodePrefab;
    public LineRenderer RouteDisplay;
    public LineRenderer OrientationHelper;

    public const string SInputGroupName = "FollowMode";
    public const float SquaredDistToReach = 1;
    public const float SquaredMaxRouteLength = 1000;

    public Route Route { get { return this.RouteHolder.Route; } }
    public UserConfig UserConfig { get { return this.RouteHolder.UserConfig; } }
    public Console Console { get { return this.RouteHolder.Console; } }

    public string InputGroupName { get { return SInputGroupName; } }
    public Dictionary<string, Action> Actions
    {
        get
        {
            return new Dictionary<string, Action>()
            {
                {
                    "SelectClosestNode", this.SelectClosestNode
                },
                {
                    "SelectPreviousNode", () =>
                    {
                        if (this.NodeIndex > 0)
                        {
                            this.NodeIndex--;
                            this.RepopulateRoute();
                        }
                    }
                },
                {
                    "SelectNextNode", this.ReachedNode
                },
                {
                    "ToggleOrientationHelper", () => this.OrientationHelper.gameObject.SetActive(!this.OrientationHelper.gameObject.activeSelf)
                }
            };
        }
    }

    private int NodeIndex
    {
        get { return this.RouteHolder.NodeIndex; }
        set { this.RouteHolder.NodeIndex = value; }
    }

    private IKeyboardMouseEvents globalHook;

    private List<NodeDisplay> nodes = new List<NodeDisplay>();
    private List<NodeDisplay> detachedNodes = new List<NodeDisplay>();

    private void Awake()
    {
        this.globalHook = Hook.GlobalEvents();
    }

    private void Start()
    {
        this.OrientationHelper.gameObject.SetActive(this.UserConfig.OrientationHelperDefault);

        this.RouteDisplay.widthMultiplier = this.UserConfig.RouteWidth;
        this.OrientationHelper.widthMultiplier = this.UserConfig.RouteWidth;
    }

    private void OnEnable()
    {
        this.RepopulateRoute();
        this.globalHook.KeyDown += this.GlobalHookKeyDown;
    }

    private void OnDisable()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
    }

    private void OnDestroy()
    {
        this.globalHook.Dispose();
    }

    private void Update()
    {
        if (this.NodeIndex < 0)
        {
            this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, this.Cursor.position });
            return;
        }

        Node next = this.Route.Nodes[this.NodeIndex];

        this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, next.Position });

        if ((next.Position - this.Cursor.position).sqrMagnitude <= SquaredDistToReach)
            this.ReachedNode();
    }

    public NodeDisplay GetNodePrefab()
    {
        return this.NodePrefab;
    }

    public LineRenderer GetRouteDisplay()
    {
        return this.RouteDisplay;
    }

    public void Reload()
    {
        this.RepopulateRoute();
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        this.Act(e.KeyCode, e.Control);
    }

    private void RepopulateRoute()
    {
        // Repopulate attached nodes.
        this.nodes.ForEach(n => Destroy(n.gameObject));
        this.nodes.Clear();
        float squaredLength = 0;
        Node previous = null;
        foreach (Node node in this.Route.Nodes.Skip(this.NodeIndex))
        {
            NodeDisplay display = this.NewNodeDisplay(false, node);
            this.nodes.Add(display);

            if (previous == null && this.isActiveAndEnabled)
                display.Select(true);

            if (node.Type == NodeType.Waypoint)
                break;

            if (previous != null)
            {
                squaredLength += (previous.Position - node.Position).sqrMagnitude;
                if (squaredLength > SquaredMaxRouteLength)
                    break;
            }

            previous = node;
        }

        // Update route display.
        Vector3[] positions = this.nodes.Select(n => n.transform.position).ToArray();
        this.RouteDisplay.positionCount = positions.Length;
        this.RouteDisplay.SetPositions(positions);

        // Update route display material.
        this.RouteDisplay.material = this.FollowMaterial;
        foreach (Node node in this.Route.Nodes.Take(this.NodeIndex).Reverse())
        {
            if (node.Type == NodeType.HeartWall)
                break;

            if (node.Type == NodeType.Heart)
            {
                this.RouteDisplay.material = this.HeartMaterial;
                break;
            }
        }

        // Repopulate detached nodes.
        this.detachedNodes.ForEach(n => Destroy(n.gameObject));
        this.detachedNodes = this.Route.DetachedNodes.Select(n => this.NewNodeDisplay(true, n)).ToList();
    }

    private void ReachedNode()
    {
        if (this.NodeIndex + 1 < this.Route.Nodes.Count)
        {
            Node reached = this.Route.Nodes[this.NodeIndex];
            if (reached.Type == NodeType.Waypoint && !string.IsNullOrEmpty(reached.WaypointCode))
            {
                Clipboard.SetText(reached.WaypointCode);
                this.Console.InfoFade("Waypoint code copied to clipboard: {0}.", reached.WaypointCode);
            }

            this.NodeIndex += 1;
            this.RepopulateRoute();
        }
    }

    private void SelectClosestNode()
    {
        if (!this.nodes.Any())
            return;

        this.NodeIndex = this.Route.Nodes
            .Select((node, i) => new { position = node.Position, i = i })
            .OrderBy(n => (this.Cursor.position - n.position).sqrMagnitude)
            .First().i;

        this.RepopulateRoute();
    }
}