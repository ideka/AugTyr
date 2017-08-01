using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public partial class EditMode : MonoBehaviour, INodeRoute, IActionable
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public NodeDisplay NodePrefab;
    public LineRenderer RouteDisplay;

    public const string SInputGroupName = "EditMode";

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
                    "AddNode", this.AddNode
                },
                {
                    "RemoveNode", this.RemoveNode
                },
                {
                    "SelectClosestNode", this.SelectClosestNode
                },
                {
                    "SelectPreviousNode", () =>
                    {
                        if (!this.onDetached)
                        {
                            if (this.nodeIndex > 0)
                                this.nodeIndex--;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectNextNode", () =>
                    {
                        if (!this.onDetached)
                        {
                            if (this.nodeIndex < this.nodes.Count - 1)
                                this.nodeIndex++;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectFirstNode", () =>
                    {
                        if (this.nodes.Any())
                        {
                            this.onDetached = false;
                            this.nodeIndex = 0;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectLastNode", () =>
                    {
                        if (this.nodes.Any())
                        {
                            this.onDetached = false;
                            this.nodeIndex = this.nodes.Count - 1;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "PreviousNodeType", () => this.ScrollNodeType(-1)
                },
                {
                    "NextNodeType", () => this.ScrollNodeType(1)
                },
                {
                    "MoveSelectedNode", this.MoveSelectedNode
                },
                {
                    "ToggleAttachNode", this.ToggleAttachNode
                },
                {
                    "GetNodeText", () =>
                    {
                        Clipboard.Clear();
                        string c = this.GetComment();
                        if (!string.IsNullOrEmpty(c))
                            Clipboard.SetText(c);
                        this.Console.InfoFade("Copied selected node text to clipboard.");
                    }
                },
                {
                    "SetNodeText", () => this.SetComment(Format(Clipboard.GetText()))
                },
                {
                    "GetNodeData", () =>
                    {
                        Clipboard.Clear();
                        string c = this.GetData();
                        if (!string.IsNullOrEmpty(c))
                            Clipboard.SetText(c);
                        this.Console.InfoFade("Copied selected node special text to clipboard.");
                    }
                },
                {
                    "SetNodeData", () => this.SetData(Format(Clipboard.GetText()))
                },
                {
                    "ToggleAttachSelection", () =>
                    {
                        this.onDetached = !this.onDetached;
                        this.UpdateSelectedNode();
                    }
                }
            };
        }
    }

    private IKeyboardMouseEvents globalHook;

    private List<NodeDisplay> nodes = new List<NodeDisplay>();
    private int nodeIndex = -1;

    private List<NodeDisplay> detachedNodes = new List<NodeDisplay>();
    private int detachedNodeIndex = -1;

    private bool onDetached = false;

    private void Awake()
    {
        this.globalHook = Hook.GlobalEvents();
    }

    private void Start()
    {
        this.RouteDisplay.widthMultiplier = this.UserConfig.RouteWidth;
    }

    private void OnEnable()
    {
        this.nodeIndex = this.RouteHolder.NodeIndex;
        this.UpdateSelectedNode();
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
        this.nodes.ForEach(n => Destroy(n.gameObject));
        this.nodes.Clear();
        this.RouteDisplay.positionCount = 0;
        this.detachedNodes.ForEach(n => Destroy(n.gameObject));
        this.detachedNodes.Clear();

        foreach (Node node in this.RouteHolder.Route.Nodes)
            this.AddVisualNode(node);
        foreach (Node detachedNode in this.RouteHolder.Route.DetachedNodes)
            this.AddVisualNode(detachedNode, true);

        this.nodeIndex = this.RouteHolder.NodeIndex;
        this.detachedNodeIndex = -1;

        if (this.isActiveAndEnabled)
            this.UpdateSelectedNode();
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        this.Act(e.KeyCode, e.Control);
    }

    public static string Format(string str)
    {
        if (str == null)
            return null;
        str = str.Trim();
        if (str == "")
            return null;
        else
            return str;
    }
}