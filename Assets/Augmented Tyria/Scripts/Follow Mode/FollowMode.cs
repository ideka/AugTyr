using System.Collections.Generic;
using UnityEngine;

public class FollowMode : MonoBehaviour
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public GameObject NodePrefab;
    public LineRenderer RouteDisplay;

    public const float SquaredDistToReach = 1;
    public const float SquaredMaxRouteLength = 100;

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