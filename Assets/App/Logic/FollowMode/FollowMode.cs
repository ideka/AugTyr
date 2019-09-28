using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowMode : MonoBehaviour, INodeRoute, IActionable
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public Material FollowMaterial;
    public Material HeartMaterial;

    public NodeDisplay NodePrefab;
    public LineRenderer RouteDisplay;
    public LineRenderer BacktrackDisplay;
    public LineRenderer OrientationHelper;

    public MonoBehaviour Holder { get => this; }
    public Route Route { get => this.RouteHolder.Route; }
    public UserConfig UserConfig { get => this.RouteHolder.UserConfig; }
    public Console Console { get => this.RouteHolder.Console; }

    public float SquaredDistToReach { get => Mathf.Pow(this.UserConfig.ReachNodeRadius, 2); }
    public float SquaredMaxRouteLength { get => Mathf.Pow(this.UserConfig.FollowMaxRouteLength, 2); }

    public string InputGroupName { get => "FollowMode"; }
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
                        if (this.SelectedNodeIndex > 0)
                        {
                            this.SelectedNodeIndex--;
                            this.RepopulateRoute();
                        }
                    }
                },
                {
                    "SelectNextNode", () => this.ReachedNode(this.SelectedNodeIndex)
                },
                {
                    "ToggleOrientationHelper", () => this.OrientationHelper.gameObject.SetActive(!this.OrientationHelper.gameObject.activeSelf)
                }
            };
        }
    }

    private int SelectedNodeIndex
    {
        get => this.RouteHolder.NodeIndex;
        set => this.RouteHolder.NodeIndex = value;
    }

    private readonly List<NodeDisplay> _nodes = new List<NodeDisplay>();
    private readonly List<NodeDisplay> _backtrackNodes = new List<NodeDisplay>();
    private readonly List<NodeDisplay> _detachedNodes = new List<NodeDisplay>();

    private void Awake()
    {
        this.SetUp();
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
    }

    private void LateUpdate()
    {
        // Get all next visible nodes with their index.
        var nextNodes = this.Route.Nodes
            .Select((node, index) => new { position = node.Position, index })
            .Skip(this.SelectedNodeIndex)
            .Take(this._nodes.Count);

        if (this.SelectedNodeIndex < 0 || !nextNodes.Any())
        {
            this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, this.Cursor.position });
            return;
        }

        this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, nextNodes.First().position });

        // Look for the first visible reached node.
        var reachedNode = nextNodes
            .Select(node => new { node.index, dist = (node.position - this.Cursor.position).sqrMagnitude })
            .Where(node => node.dist <= this.SquaredDistToReach)
            .FirstOrDefault();

        if (reachedNode != null)
            this.ReachedNode(reachedNode.index);
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

    private void RepopulateRoute()
    {
        // Repopulate attached nodes.
        {
            this._nodes.ForEach(n => Destroy(n.gameObject));
            this._nodes.Clear();

            float squaredLength = 0;
            Node previous = null;
            foreach (Node node in this.Route.Nodes.Skip(this.SelectedNodeIndex))
            {
                NodeDisplay display = this.NewNodeDisplay(false, node);
                this._nodes.Add(display);

                if (previous == null && this.isActiveAndEnabled)
                    display.Select(true);

                if (node.Type == NodeType.Teleport)
                    break;

                if (previous != null)
                {
                    squaredLength += (previous.Position - node.Position).sqrMagnitude;
                    if (squaredLength > this.SquaredMaxRouteLength && this._nodes.Count >= this.UserConfig.MinDisplayNodeCount)
                        break;
                }

                previous = node;
            }
        }

        // Update route display.
        {
            Vector3[] positions = this._nodes.Select(n => n.transform.position).ToArray();
            this.RouteDisplay.positionCount = positions.Length;
            this.RouteDisplay.SetPositions(positions);
        }

        // Update route display material.
        this.RouteDisplay.material = this.FollowMaterial;
        foreach (Node node in this.Route.Nodes.Take(this.SelectedNodeIndex).Reverse())
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
        this._detachedNodes.ForEach(n => Destroy(n.gameObject));
        this._detachedNodes.Clear();
        this._detachedNodes.AddRange(this.Route.DetachedNodes.Select(n => this.NewNodeDisplay(true, n)));

        // Update backtrack.
        if (this.UserConfig.ShowFollowBacktrack)
        {
            // Repopulate backtrack nodes.
            this._backtrackNodes.ForEach(n => Destroy(n.gameObject));
            this._backtrackNodes.Clear();

            float squaredLength = 0;
            Node previous = null;
            foreach (var iter in this.Route.Nodes.Take(this.SelectedNodeIndex).Reverse().Select((node, i) => new { node, i }))
            {
                // Don't go past teleport nodes.
                if (iter.node.Type == NodeType.Teleport && iter.i > 0)
                    break;

                NodeDisplay display = this.NewNodeDisplay(false, iter.node);
                display.SetReached();
                this._backtrackNodes.Add(display);

                // Only start counting length from 2nd node to ensure nodes won't just disappear
                // when consuming a single node with a very long trail after it.
                if (iter.i > 1)
                {
                    squaredLength += (previous.Position - iter.node.Position).sqrMagnitude;
                    if (squaredLength > this.SquaredMaxRouteLength)
                        break;
                }

                previous = iter.node;
            }

            // Update backtrack route display.
            Vector3[] positions = _backtrackNodes.Select(n => n.transform.position).ToArray();
            this.BacktrackDisplay.positionCount = positions.Length;
            this.BacktrackDisplay.SetPositions(positions);
        }
    }

    private void ReachedNode(int reachedNodeIndex)
    {
        if (reachedNodeIndex + 1 < this.Route.Nodes.Count)
        {
            Node reached = this.Route.Nodes[reachedNodeIndex];
            if (reached.Type == NodeType.Teleport && !string.IsNullOrEmpty(reached.WaypointCode))
            {
                GUIUtility.systemCopyBuffer = reached.WaypointCode;
                this.Console.InfoFade("Waypoint code copied to clipboard: {0}.", reached.WaypointCode);
            }

            this.SelectedNodeIndex = reachedNodeIndex + 1;
            this.RepopulateRoute();
        }
    }

    private void SelectClosestNode()
    {
        var closestNode = this.Route.Nodes
            .Select((node, index) => new { position = node.Position, index })
            .OrderBy(node => (this.Cursor.position - node.position).sqrMagnitude)
            .FirstOrDefault();

        if (closestNode == null)
            return;

        this.SelectedNodeIndex = closestNode.index;

        this.RepopulateRoute();
    }
}