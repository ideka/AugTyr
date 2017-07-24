using Gma.System.MouseKeyHook;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

public partial class EditMode : MonoBehaviour
{
    public Transform Cursor;
    public RouteHolder RouteHolder;

    public GameObject NodePrefab;
    public LineRenderer RouteDisplay;

    public Route Route { get { return this.RouteHolder.Route; } }

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

        this.UpdateSelectedNode();
    }

    private void GlobalHookKeyDown(object sender, KeyEventArgs e)
    {
        if (Camera.main.cullingMask == 0)
            return;

        switch (e.KeyCode)
        {
            // Add/remove nodes.
            case Keys.Add:
                this.AddNode();
                break;

            case Keys.Subtract:
                this.RemoveNode();
                break;

            // Select closest node.
            case Keys.NumPad5:
                this.SelectClosestNode();
                break;

            // Change selected node.
            case Keys.NumPad4:
                if (e.Control)
                {
                    if (this.nodes.Any())
                    {
                        this.onDetached = false;
                        this.nodeIndex = 0;
                        this.UpdateSelectedNode();
                    }
                }
                else if (!this.onDetached)
                {
                    if (this.nodeIndex > 0)
                        this.nodeIndex--;
                    this.UpdateSelectedNode();
                }
                break;

            case Keys.NumPad6:
                if (e.Control)
                {
                    if (this.nodes.Any())
                    {
                        this.onDetached = false;
                        this.nodeIndex = this.nodes.Count - 1;
                        this.UpdateSelectedNode();
                    }
                }
                else if (!this.onDetached)
                {
                    if (this.nodeIndex < this.nodes.Count - 1)
                        this.nodeIndex++;
                    this.UpdateSelectedNode();
                }
                break;

            // Change node type.
            case Keys.NumPad8:
                this.ScrollNodeType(-1);
                break;

            case Keys.NumPad2:
                this.ScrollNodeType(1);
                break;

            // Move node.
            case Keys.NumPad7:
                this.MoveSelectedNode();
                break;

            // Attach/detach node.
            case Keys.NumPad9:
                this.ToggleAttachNode();
                break;

            // Set/get node data.
            case Keys.NumPad1:
                if (e.Control)
                {
                    this.SetComment(Format(Clipboard.GetText()));
                }
                else
                {
                    Clipboard.Clear();
                    string c = this.GetComment();
                    if (!string.IsNullOrEmpty(c))
                        Clipboard.SetText(c);
                }
                break;

            case Keys.NumPad3:
                if (e.Control)
                {
                    this.SetData(Format(Clipboard.GetText()));
                }
                else
                {
                    Clipboard.Clear();
                    string c = this.GetData();
                    if (!string.IsNullOrEmpty(c))
                        Clipboard.SetText(c);
                }
                break;

            // Force change mode.
            case Keys.NumPad0:
                this.onDetached = !this.onDetached;
                this.UpdateSelectedNode();
                break;
        }
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