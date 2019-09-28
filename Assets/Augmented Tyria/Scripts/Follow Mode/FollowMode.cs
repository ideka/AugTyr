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
    public LineRenderer OrientationHelper;


    public MonoBehaviour Holder { get { return this; } }
    public Route Route { get { return this.RouteHolder.Route; } }
    public UserConfig UserConfig { get { return this.RouteHolder.UserConfig; } }
    public Console Console { get { return this.RouteHolder.Console; } }

	public float SquaredDistToReach { get { return UserConfig.ReachNodeRadius * UserConfig.ReachNodeRadius; } }
	public float SquaredMaxRouteLength { get { return UserConfig.FollowMaxRouteLength * UserConfig.FollowMaxRouteLength; } }

	public string InputGroupName { get { return "FollowMode"; } }
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
                        if (this.NextNodeIndex > 0)
                        {
                            this.NextNodeIndex--;
                            this.RepopulateRoute();
                        }
                    }
                },
                {
                    "SelectNextNode", () => this.ReachedNode( NextNodeIndex + 1 )
				},
                {
                    "ToggleOrientationHelper", () => this.OrientationHelper.gameObject.SetActive(!this.OrientationHelper.gameObject.activeSelf)
                }
            };
        }
    }

    private int NextNodeIndex
    {
        get { return this.RouteHolder.NodeIndex; }
        set { this.RouteHolder.NodeIndex = value; }
    }

    private List<NodeDisplay> nodes = new List<NodeDisplay>();
    private List<NodeDisplay> detachedNodes = new List<NodeDisplay>();

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

    private void Update()
    {
        if (this.NextNodeIndex < 0)
        {
            this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, this.Cursor.position });
            return;
        }

		if( nodes == null )
			return;

		//Rejoin
		var nextNodes = Route.Nodes.Skip( NextNodeIndex ).Take( nodes.Count ).ToList();
		OrientationHelper.SetPositions( new Vector3[] { this.Cursor.position, nextNodes.First().Position } );

		var next = nextNodes
			.Select( ( n, i ) => new { node = n, index = i, dist = ( n.Position - this.Cursor.position ).sqrMagnitude } )
			.Where( n => n.dist <= SquaredDistToReach )
			.OrderBy( n => n.dist )
			.FirstOrDefault();

		if( next != null )
			ReachedNode( NextNodeIndex + next.index );
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
        this.nodes.ForEach(n => Destroy(n.gameObject));
        this.nodes.Clear();
        float squaredLength = 0;
        Node previous = null;
        foreach (Node node in this.Route.Nodes.Skip(this.NextNodeIndex))
        {
            NodeDisplay display = this.NewNodeDisplay(false, node);
            this.nodes.Add(display);

            if (previous == null && this.isActiveAndEnabled)
                display.Select(true);

            if (node.Type == NodeType.Teleport)
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
        foreach (Node node in this.Route.Nodes.Take(this.NextNodeIndex).Reverse())
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

	private void ReachedNode( int reachedNodeIndex ) {
		if( reachedNodeIndex + 1 < this.Route.Nodes.Count ) {
			Node reached = this.Route.Nodes[reachedNodeIndex];
			if( reached.Type == NodeType.Teleport && !string.IsNullOrEmpty( reached.WaypointCode ) ) {
				GUIUtility.systemCopyBuffer = reached.WaypointCode;
				this.Console.InfoFade( "Waypoint code copied to clipboard: {0}.", reached.WaypointCode );
			}

			this.NextNodeIndex = reachedNodeIndex + 1;
			this.RepopulateRoute();
		}
	}

	private void SelectClosestNode()
    {
        if (!this.nodes.Any())
            return;

        this.NextNodeIndex = this.Route.Nodes
            .Select((node, i) => new { position = node.Position, i = i })
            .OrderBy(n => (this.Cursor.position - n.position).sqrMagnitude)
            .First().i;

        this.RepopulateRoute();
    }
}