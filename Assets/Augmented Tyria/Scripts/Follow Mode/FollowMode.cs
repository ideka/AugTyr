using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public class FollowMode : MonoBehaviour
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public GameObject NodePrefab;
    public LineRenderer RouteDisplay;

    public LineRenderer OrientationHelper;

    public const float SquaredDistToReach = 1;
    public const float SquaredMaxRouteLength = 1000;

    public Route Route { get { return this.RouteHolder.Route; } }

    private int nodeIndex
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

    private void OnEnable()
    {
        this.RepopulateRoute();
        this.OrientationHelper.gameObject.SetActive(false);
        this.globalHook.KeyDown += this.GlobalHookKeyDown;
        this.globalHook.KeyUp += this.GlobalHookKeyUp;
    }

    private void OnDisable()
    {
        this.globalHook.KeyDown -= this.GlobalHookKeyDown;
        this.globalHook.KeyUp -= this.GlobalHookKeyUp;
    }

    private void OnDestroy()
    {
        this.globalHook.Dispose();
    }

    private void Update()
    {
        if (this.nodeIndex < 0)
            return;

        Node next = this.Route.Nodes[this.nodeIndex];

        this.OrientationHelper.SetPositions(new Vector3[] { this.Cursor.position, next.Position });

        if ((next.Position - this.Cursor.position).sqrMagnitude <= SquaredDistToReach)
            this.ReachedNode();
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        switch (e.KeyCode)
        {
            // Manually change nodes.
            case Keys.NumPad4:
                if (this.nodeIndex > 0)
                {
                    this.nodeIndex--;
                    this.RepopulateRoute();
                }
                break;

            case Keys.NumPad6:
                this.ReachedNode();
                break;

            // Display the orientation helper.
            case Keys.NumPad0:
                this.OrientationHelper.gameObject.SetActive(true);
                break;
        }
    }

    private void GlobalHookKeyUp(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.NumPad0:
                this.OrientationHelper.gameObject.SetActive(false);
                break;
        }
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

            if (previous == null)
                display.Select(true);
            else
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
            Node reached = this.Route.Nodes[this.nodeIndex];
            if (reached.Type == NodeType.Waypoint)
            {
                Clipboard.Clear();
                if (reached.WaypointCode != null && reached.WaypointCode != "")
                    Clipboard.SetText(reached.WaypointCode);
            }

            this.nodeIndex += 1;
            this.RepopulateRoute();
        }
    }
}