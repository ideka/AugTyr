using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EditMode : MonoBehaviour, INodeRoute, IActionable
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public NodeDisplay NodePrefab;
    public LineRenderer RouteDisplay;

    public MonoBehaviour Holder { get => this; }
    public Route Route { get => this.RouteHolder.Route; }
    public UserConfig UserConfig { get => this.RouteHolder.UserConfig; }
    public Console Console { get => this.RouteHolder.Console; }

    public string InputGroupName { get => "EditMode"; }
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
                        if (!this._onDetached)
                        {
                            if (this._nodeIndex > 0)
                                this._nodeIndex--;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectNextNode", () =>
                    {
                        if (!this._onDetached)
                        {
                            if (this._nodeIndex < this._nodes.Count - 1)
                                this._nodeIndex++;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectFirstNode", () =>
                    {
                        if (this._nodes.Any())
                        {
                            this._onDetached = false;
                            this._nodeIndex = 0;
                            this.UpdateSelectedNode();
                        }
                    }
                },
                {
                    "SelectLastNode", () =>
                    {
                        if (this._nodes.Any())
                        {
                            this._onDetached = false;
                            this._nodeIndex = this._nodes.Count - 1;
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
                        GUIUtility.systemCopyBuffer = this.GetComment();
                        this.Console.InfoFade("Copied selected node text to clipboard.");
                    }
                },
                {
                    "SetNodeText", () => this.SetComment(Format(GUIUtility.systemCopyBuffer))
                },
                {
                    "GetNodeData", () =>
                    {
                        GUIUtility.systemCopyBuffer = this.GetData();
                        this.Console.InfoFade("Copied selected node special text to clipboard.");
                    }
                },
                {
                    "SetNodeData", () => this.SetData(Format(GUIUtility.systemCopyBuffer))
                },
                {
                    "ToggleAttachSelection", () =>
                    {
                        this._onDetached = !this._onDetached;
                        this.UpdateSelectedNode();
                    }
                }
            };
        }
    }

    private readonly List<NodeDisplay> _nodes = new List<NodeDisplay>();
    private int _nodeIndex = -1;

    private readonly List<NodeDisplay> _detachedNodes = new List<NodeDisplay>();
    private int _detachedNodeIndex = -1;

    private bool _onDetached = false;

    private void Awake()
    {
        this.SetUp();
    }

    private void Start()
    {
        this.RouteDisplay.widthMultiplier = this.UserConfig.RouteWidth;
    }

    private void OnEnable()
    {
        this._nodeIndex = this.RouteHolder.NodeIndex;
        this.UpdateSelectedNode();
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
        this._nodes.ForEach(n => Destroy(n.gameObject));
        this._nodes.Clear();
        this.RouteDisplay.positionCount = 0;
        this._detachedNodes.ForEach(n => Destroy(n.gameObject));
        this._detachedNodes.Clear();

        foreach (Node node in this.RouteHolder.Route.Nodes)
            this.AddVisualNode(node);
        foreach (Node detachedNode in this.RouteHolder.Route.DetachedNodes)
            this.AddVisualNode(detachedNode, true);

        this._nodeIndex = this.RouteHolder.NodeIndex;
        this._detachedNodeIndex = -1;

        if (this.isActiveAndEnabled)
            this.UpdateSelectedNode();
    }

    public static string Format(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return null;

        return str.Trim();
    }
}