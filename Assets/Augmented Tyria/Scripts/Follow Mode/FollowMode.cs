using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowMode : MonoBehaviour
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public GameObject NodePrefab;
    public LineRenderer RouteDisplay;

    public const float SquaredDistToReach = 1;
    public const float SquaredMaxRouteLength = 1000;

    public Route Route { get { return this.RouteHolder.Route; } }

    private int nodeIndex
    {
        get { return this.RouteHolder.NodeIndex; }
        set { this.RouteHolder.NodeIndex = value; }
    }

    private List<NodeDisplay> nodes = new List<NodeDisplay>();
    private List<NodeDisplay> detachedNodes = new List<NodeDisplay>();

    private void OnEnable()
    {
        this.RepopulateRoute();
    }

    private void Update()
    {
        if (this.nodeIndex < 0)
            return;

        if ((this.Route.Nodes[this.nodeIndex].Position - this.Cursor.position).sqrMagnitude <= SquaredDistToReach)
            this.ReachedNode();
    }

    private void RepopulateRoute()
    {
        // Repopulate attached nodes.
        this.nodes.ForEach(n => Destroy(n.gameObject));
        this.nodes.Clear();
        float squaredLength = 0;
        Node previous = null;
        foreach (Node node in this.Route.Nodes.Skip(this.nodeIndex))
        {
            if (squaredLength > SquaredMaxRouteLength)
                break;

            GameObject gameObject = (GameObject)Instantiate(this.NodePrefab, this.RouteDisplay.transform);
            NodeDisplay display = gameObject.GetComponent<NodeDisplay>();
            display.Node = node;
            display.SetMesh(false);
            this.nodes.Add(display);

            if (node.Type == NodeType.Waypoint)
                break;

            if (previous != null)
                squaredLength += (previous.Position - node.Position).sqrMagnitude;
            previous = node;
        }

        // Update route display.
        this.RouteDisplay.numPositions = 0;  // TODO: Find out if this is needed.
        Vector3[] positions = this.nodes.Select(n => n.transform.position).ToArray();
        this.RouteDisplay.numPositions = positions.Length;
        this.RouteDisplay.SetPositions(positions);

        // Repopulate detached nodes.
        this.detachedNodes.ForEach(n => Destroy(n.gameObject));
        this.detachedNodes.Clear();
        foreach (Node node in this.Route.DetachedNodes)
        {
            GameObject gameObject = (GameObject)Instantiate(this.NodePrefab, this.RouteDisplay.transform);
            NodeDisplay display = gameObject.GetComponent<NodeDisplay>();
            display.Node = node;
            display.SetMesh(true);
            this.detachedNodes.Add(display);
        }
    }

    private void ReachedNode()
    {
        if (this.nodeIndex + 1 < this.Route.Nodes.Count)
        {
            this.nodeIndex += 1;
            this.RepopulateRoute();
        }
    }
}