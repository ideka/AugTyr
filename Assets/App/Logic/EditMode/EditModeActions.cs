using System;
using System.Linq;
using UnityEngine;

public partial class EditMode
{
    private Node GetSelectedNode()
    {
        if (this._onDetached)
        {
            if (this._detachedNodeIndex < 0)
                return null;
            else
                return this.Route.DetachedNodes[this._detachedNodeIndex];
        }
        else
        {
            if (this._nodeIndex < 0)
                return null;
            else
                return this.Route.Nodes[this._nodeIndex];
        }
    }

    private void AddVisualNode(Node node, bool detached = false, int at = -1)
    {
        NodeDisplay display = this.NewNodeDisplay(detached, node);

        if (detached)
        {
            this._detachedNodes.Add(display);
        }
        else
        {
            if (at == -1)
                this._nodes.Add(display);
            else
                this._nodes.Insert(at, display);

            this.UpdatePath();
        }
    }

    private void RemoveVisualNode(int at, bool detached = false)
    {
        if (detached)
        {
            Destroy(this._detachedNodes[at].gameObject);
            this._detachedNodes.RemoveAt(at);
        }
        else
        {
            Destroy(this._nodes[at].gameObject);
            this._nodes.RemoveAt(at);
            this.UpdatePath();
        }
    }

    private void UpdatePath()
    {
        Vector3[] positions = this.Route.Nodes.Select(n => n.Position).ToArray();
        this.RouteDisplay.positionCount = positions.Length;
        this.RouteDisplay.SetPositions(positions);
    }

    private void ScrollNodeType(int amount)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        // `(x % len + len) % len` pattern normalizes x between [0, len).
        int total = Enum.GetValues(typeof(NodeType)).Length;
        node.Type = (NodeType)((((int)node.Type + amount) % total + total) % total);

        this.UpdateSelectedNodeInfo();
    }

    private string GetComment()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return null;
        return node.Comment;
    }

    private void SetComment(string to)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        node.Comment = to;
        this.UpdateSelectedNodeInfo();
    }

    private string GetData()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return null;

        switch (node.Type)
        {
            case NodeType.Teleport:
                return node.WaypointCode;

            case NodeType.HeartWall:
                return node.HeartWallValue;
        }
        return null;
    }

    private void SetData(string to)
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        switch (node.Type)
        {
            case NodeType.Teleport:
                node.SetWaypointCode(this.RouteHolder.MapId, this.RouteHolder.GameDatabase, to);
                break;

            case NodeType.HeartWall:
                node.SetHeartWallValue(to);
                break;
        }
        this.UpdateSelectedNodeInfo();
    }

    private void UpdateSelectedNodeInfo()
    {
        if (this._onDetached && this._detachedNodeIndex >= 0)
            this._detachedNodes[this._detachedNodeIndex].Node = this.Route.DetachedNodes[this._detachedNodeIndex];
        else if (!this._onDetached && this._nodeIndex >= 0)
            this._nodes[this._nodeIndex].Node = this.Route.Nodes[this._nodeIndex];
    }

    private void ToggleAttachNode()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        if (this._onDetached)
        {
            this.Route.DetachedNodes.RemoveAt(this._detachedNodeIndex);
            this.Route.Nodes.Insert(this._nodeIndex + 1, node);
            this.RemoveVisualNode(this._detachedNodeIndex, true);
            this.AddVisualNode(node, false, this._nodeIndex + 1);

            this._detachedNodeIndex = -1;
            this._nodeIndex++;
            this._onDetached = false;

            this.UpdateSelectedNode();
            this.UpdatePath();
        }
        else
        {
            this.Route.Nodes.RemoveAt(this._nodeIndex);
            this.Route.DetachedNodes.Add(node);
            this.RemoveVisualNode(this._nodeIndex);
            this.AddVisualNode(node, true);

            this._detachedNodeIndex = this.Route.DetachedNodes.Count - 1;
            if (this._nodeIndex > 0 || !this._nodes.Any())
                this._nodeIndex--;
            this._onDetached = true;

            this.UpdateSelectedNode();
            this.UpdatePath();
        }
    }

    private void SelectClosestNode()
    {
        var closest = this._nodes
            .Select((display, i) => new { display.transform, attached = true, i })
            .Concat(this._detachedNodes.Select((display, i) => new { display.transform, attached = false, i }))
            .OrderBy(n => (this.Cursor.position - n.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (closest == null)
            return;

        this._onDetached = !closest.attached;
        if (this._onDetached)
            this._detachedNodeIndex = closest.i;
        else
            this._nodeIndex = closest.i;

        this.UpdateSelectedNode();
    }

    private void AddNode()
    {
        var newNode = new Node()
        {
            Type = NodeType.Reach,
            Position = this.Cursor.position
        };

        if (_onDetached)
        {
            this.Route.DetachedNodes.Add(newNode);
            this.AddVisualNode(newNode, true);
            this._detachedNodeIndex = this._detachedNodes.Count - 1;
        }
        else
        {
            this._nodeIndex++;
            this.Route.Nodes.Insert(this._nodeIndex, newNode);
            this.AddVisualNode(newNode, false, this._nodeIndex);
        }

        this.UpdateSelectedNode();
    }

    private void RemoveNode()
    {
        if (this.GetSelectedNode() == null)
            return;

        if (_onDetached)
        {
            this.Route.DetachedNodes.RemoveAt(this._detachedNodeIndex);
            this.RemoveVisualNode(this._detachedNodeIndex, true);
            this._detachedNodeIndex = -1;
        }
        else
        {
            this.Route.Nodes.RemoveAt(this._nodeIndex);
            this.RemoveVisualNode(this._nodeIndex);
            if (this._nodeIndex != 0 || !this._nodes.Any())
                this._nodeIndex--;
        }
        this.UpdateSelectedNode();
    }

    private void MoveSelectedNode()
    {
        Node node = this.GetSelectedNode();
        if (node == null)
            return;

        node.Position = this.Cursor.position;
        this.UpdateSelectedNodeInfo();
        this.UpdatePath();
    }

    private void UpdateSelectedNode()
    {
        this.RouteHolder.NodeIndex = this._nodeIndex;

        this._nodes.ForEach(n => n.Select(false));
        this._detachedNodes.ForEach(n => n.Select(false));

        if (this._onDetached)
        {
            if (this._detachedNodeIndex >= 0)
                this._detachedNodes[this._detachedNodeIndex].Select(true);
        }
        else if (this._nodeIndex >= 0)
        {
            this._nodes[this._nodeIndex].Select(true);
        }
    }
}